using PMDDesktop.Exceptions;
using PMDDesktop.Utils;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Holds methods and fields related to SaveData management. It lives in its own static class to declutter the primary SaveData class.
/// </summary>
public class SaveDataManager : ISaveDataIndexable, IEnumerable<SaveData>
{

	/// <summary>
	/// <para>Creates a SaveDataManager.</para>
	/// <para>Typically, this object is a singleton that'll be passed around; however there are no checks on having multiple.</para>
	/// </summary>
	public SaveDataManager()
	{

	}

	/// <summary>
	/// <para>Does this SaveDataManager actually write to files?</para>
	/// <para>If this is false, file writes to the <b>filesystem/disk</b> will be skipped.</para>
	/// </summary>
	/// <remarks>
	/// This is property as "false" is very useful in unit testing. Should probably be "true" during runtime.
	/// </remarks>
	public bool WritingEnabled { get; private set; }

	/// <summary>
	/// Is the SaveDataManager currently flushing?
	/// </summary>
	public bool IsFlushing { get; private set; }

	/// <summary>
	/// This lists all known save datas. SaveDatas in this list will be periodically written to disk if their "Dirty" property is true.
	/// </summary>
	private readonly Dictionary<Guid, SaveData> saveDatas = [];

	/// <summary>
	/// Save Datas queued for deletion. SaveDatas in this list will be deleted when changes are flushed.
	/// </summary>
	private readonly Queue<SaveData> deleteQueue = [];

	/// <summary>
	/// Cache for save paths so they don't need to be solved by reflection each time. Also helps with validation that there's no duplicates in the dataset.
	/// </summary>
	private readonly Dictionary<Type, string> savePathCache = [];

	/// <summary>
	/// Loads from the 'save' directory, and enables writing.
	/// </summary>
	/// <remarks>
	/// This can throw under many, many circumstances. If it does, cancel all operations and make sure the error is conveyed to the user.
	/// </remarks>
	public async Task LoadFromFilesAndEnableWriting()
	{

		string dirPath = Path.Combine(AppContext.BaseDirectory, "save");

		if (!Directory.Exists(dirPath))
			Directory.CreateDirectory(dirPath);

		await LoadAllSaveData();

		WritingEnabled = true;

	}

	/// <summary>
	/// Get a SaveData by type and GUID. Might be null if none is found.
	/// </summary>
	/// <typeparam name="T">The type to get.</typeparam>
	/// <param name="GUID">The GUID of the object you're looking for.</param>
	/// <returns>Will return the object if found, otherwise will return null.</returns>
	public T? GetSave<T>(Guid GUID) where T : SaveData
	{

		return GetByGUID(GUID) as T;

	}

	/// <summary>
	/// Try to get a save data by type and GUID. Will return false and data will be null if not found.
	/// </summary>
	/// <typeparam name="T">The type of the save data to get.</typeparam>
	/// <param name="GUID">The GUID of the object you're looking for.</param>
	/// <param name="data">The found save data, if any was found. Will be null if the return was false.</param>
	/// <returns>True if the data is found.</returns>
	public bool TryGetSave<T>(Guid GUID, [NotNullWhen(true)] out T? data) where T : SaveData
	{

		data = GetSave<T>(GUID);

		return data is not null;

	}

	/// <summary>
	/// Internal function for checking if a GUID is unused.
	/// </summary>
	/// <param name="GUID">The GUID you're checking the uniqueness of.</param>
	/// <returns>true if this is unique, false if it is duplicated</returns>
	internal bool IsUUIDFree(Guid GUID)
	{

		if (GetByGUID(GUID) != null)
			return false;

		// Make sure we're not about to delete this GUID either.
		foreach (SaveData deleteItem in deleteQueue)
			if (deleteItem.GUID == GUID)
				return false;

		return true;

	}

	/// <summary>
	/// <para>Internal function for getting a SaveData object by GUID.</para>
	/// <para>Either returns the SaveData if it was found, or null if it was not.</para>
	/// </summary>
	/// <param name="GUID">The GUID of the SaveData to attempt to get.</param>
	/// <returns>The target SaveData object, or null.</returns>
	internal SaveData? GetByGUID(Guid GUID)
	{

		saveDatas.TryGetValue(GUID, out SaveData? data);

		return data;

	}

	/// <summary>
	/// <para>This function scans the save folder and attempts to load all the SaveData in that folder and create objects for them.</para>
	/// <para>It should be called once during initialization of a WritingEnabled SaveManager, and never again.</para>
	/// <para>It should not be called in situations like Unit Testing, it is intended for loading/saving server state.</para>
	/// </summary>
	/// <remarks>
	/// <para>This function may throw in many different ways. Try your best to clearly communicate the thrown error to the user.</para>
	/// <para>Continuing to use the SaveManager after this function throws may result in data corruption or loss.</para>
	/// </remarks>
	private async Task LoadAllSaveData()
	{

		// This should find all classes implementing "SaveData."
		IEnumerable<Type> saveTypes = TypeUtils.GetInstanceableClassesAssignableTo(typeof(SaveData));

		foreach (Type type in saveTypes)
		{

			string directory = GetDirectoryPath(type);

			// The directory may not exist.
			if (!Directory.Exists(directory))
				continue;

			foreach (string filePath in Directory.EnumerateFiles(directory, "*.json"))
			{

				string name = Path.GetFileNameWithoutExtension(filePath);

				// This is not okay. All JSON files should be strictly save files in these directories.
				if (!Guid.TryParse(name, out Guid loadedGUID))
					throw new Exception($"Couldn't parse {name} as a GUID at {filePath}");

				using FileStream readStream = File.OpenRead(filePath);

				object deserialized = JsonSerializer.Deserialize(readStream, type, AppInfo.JSON_OPTIONS)
					?? throw new Exception($"Deserialized save data from {filePath} was null.");

				if (deserialized is not SaveData data)
					throw new InvalidCastException($"{deserialized} couldn't be cast to SaveData");

				data.GUID = loadedGUID;

				Add(data);

				data.Dirty = false;

			}

		}

	}

	/// <summary>
	/// <para>Asynchronously saves all unsaved SaveData objects, one-by-one. May take time.</para>
	/// <para>Will also delete any SaveData queued for deletion.</para>
	/// <para>Try not to call while SaveData is being updated; saving won't fail but it might make continuity issues.</para>
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// This can only be called once at a time. Another attempt at concurrent saving/flushing will throw the new one.
	/// </remarks>
	/// <exception cref="InvalidOperationException">Throws if this function is SaveManager is already flushing/saving.</exception>
	public async Task SaveAllChanges()
	{

		if (IsFlushing)
			throw new InvalidOperationException("SavaDataManager is already flushing!");

		IsFlushing = true;

		try
		{

			// Deleting first means if a SaveData GUID is duplicated between save & delete,
			// it'll save after being deleted, meaning no data loss.
			// That should never happen though!!! >:(
			while (deleteQueue.TryDequeue(out SaveData? deleteData))
			{

				if (deleteData == null) // Okay???
					throw new NullReferenceException($"While wiping the deletion queue ({deleteQueue}), found a null. This might indicate a problem somewhere else in the program.");

				await EraseSaveDataFile(deleteData);

			}

			foreach (var data in saveDatas.Values)
			{

				if (data.Dirty)
					await SaveDataToFile(data);

			}

		}
		finally
		{

			IsFlushing = false;

		}

	}

	/// <summary>
	/// <para>Returns the folder path that this type would be saved to.</para>
	/// </summary>
	/// <param name="type">The type you're looking for the folder path of.</param>
	/// <returns>The folder path of the type.</returns>
	/// <remarks>It is possible to pass types that wouldn't be saved in this function. It will still return, but the result is meaningless.</remarks>
	private string GetDirectoryPath(Type type)
	{

		// The real reason the list exists is to check for duplicates, but we can still use it as cache!
		if (savePathCache.TryGetValue(type, out string? cachedValue))
			return cachedValue;

		SaveSubdirectoryAttribute? folderAttribute = type.GetCustomAttribute<SaveSubdirectoryAttribute>()
			?? throw new MissingAttributeException(type, typeof(SaveSubdirectoryAttribute));

		string subdirectoryName = folderAttribute.subdirectoryName;
		string resultPath = Path.Combine(AppContext.BaseDirectory, "save", subdirectoryName);

		// Check to make sure this isn't duplicating another path.
		foreach (KeyValuePair<Type, string> existingValue in savePathCache)
			if (existingValue.Value == resultPath)
				throw new DuplicateAttributeDataException(type, existingValue.Key, resultPath, typeof(SaveSubdirectoryAttribute));

		savePathCache.Add(type, resultPath);

		return resultPath;

	}

	/// <summary>
	/// Returns the folder path that this type would be saved to.
	/// </summary>
	/// <param name="data">The data you're looking for the folder path of.</param>
	/// <returns>The folder path of the type.</returns>
	private string GetDirectoryPath(SaveData data)
	{

		return GetDirectoryPath(data.GetType());

	}

	/// <summary>
	/// Returns the exact path (ends with <b>.json</b>) that a SaveData object is expecting to be serialized and written to.
	/// </summary>
	/// <param name="data">The data you're looking for the file path for.</param>
	/// <returns>The file path that the SaveData's data should be saved to.</returns>
	private string GetFilePath(SaveData data)
	{

		return Path.Combine(GetDirectoryPath(data.GetType()), data.GUID + ".json");

	}

	/// <summary>
	/// Serializes the data to JSON writes it to a file. Should be called when flushing changes.
	/// </summary>
	/// <param name="data">The data to save</param>
	/// <returns></returns>
	/// <remarks>
	/// <para>If WritingEnabled is false, it will instead write to a null, but it will still serialize the data.</para>
	/// </remarks>
	private async Task SaveDataToFile(SaveData data)
	{

		if (WritingEnabled)
		{

			string dirPath = GetDirectoryPath(data);

			if (!Directory.Exists(dirPath))
				Directory.CreateDirectory(dirPath);
			
		}

		await using Stream stream = WritingEnabled
			? File.Create(GetFilePath(data))
			: Stream.Null;

		await JsonSerializer.SerializeAsync(stream, data, data.GetType(), AppInfo.JSON_OPTIONS);

		data.Dirty = false;

	}

	/// <summary>
	/// Deletes the file that a data would've saved to. Should be called when flushing changes.
	/// </summary>
	/// <param name="data">The data to delete.</param>
	/// <returns>Resolves Task once file is deleted.</returns>
	private async Task EraseSaveDataFile(SaveData data)
	{

		if (!WritingEnabled)
			return;

		string path = GetFilePath(data);

		// There's a chance we haven't had the chance to flush/save yet, so the file may not exist.
		if (File.Exists(path))
			File.Delete(path);

	}

	/// <summary>
	/// <para>Begin tracking this object and saving it.</para>
	/// </summary>
	/// <param name="data">The SaveData to begin tracking and saving.</param>
	/// <exception cref="InvalidOperationException">Throws if the GUID is already being used in the SaveManager or if the SaveData already exists in the SaveManager.</exception>
	public void Add(SaveData data)
	{

		if (saveDatas.TryGetValue(data.GUID, out SaveData? blockingData))
		{

			if (blockingData == data)
				throw new InvalidOperationException($"{data} is already in the SaveDataManager!");
			else
				throw new InvalidOperationException($"{data} (trying to be added) has the same UUID as {blockingData} (already added) in the SaveDataManager!");

		}

		data.Dirty = true;
		saveDatas.Add(data.GUID, data);

		Console.WriteLine($"Added SaveData ({data.GetType().Name}): {data}");

	}

	/// <summary>
	/// <para>Unlink this SaveData and queue the deletion of the file it is saving to.</para>
	/// </summary>
	/// <remarks>
	/// <para>The SaveData object will still exist until the .NET garbage collector collects it.</para>
	/// <para>The file SaveData will be removed from the list immediately; but the file won't be deleted until the next "flush" happens.</para>
	/// </remarks>
	public void Remove(SaveData data)
	{

		if (saveDatas.TryGetValue(data.GUID, out SaveData? matchingData))
		{

			if (matchingData == data)
			{
				saveDatas.Remove(data.GUID);
				deleteQueue.Enqueue(data);
				return;
			}
			else
				throw new InvalidOperationException($"{data} (trying to be deleted) has the same UUID as {matchingData} (different added object) in the SaveDataManager, but is a different object!");

		}

		throw new InvalidOperationException($"{data} is trying to be deleted from SaveDataManager, but isn't in the SaveDataManager!");

	}

	/// <summary>
	/// Returns an IEnumerator<SaveData> that iterates through all SaveDatas in the SaveDataManager.
	/// </summary>
	/// <returns>An IEnumerator<SaveData> that iterates through all SavaData objects in the SaveDataManager.</returns>
	public IEnumerator<SaveData> GetEnumerator()
	{
		return saveDatas.Values.GetEnumerator();
	}

	/// <summary>
	/// Returns an IEnumerator that iterates through all SaveDatas in the SaveDataManager.
	/// </summary>
	/// <returns>An IEnumerator that iterates through all SavaData objects in the SaveDataManager.</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return saveDatas.Values.GetEnumerator();
	}

}
