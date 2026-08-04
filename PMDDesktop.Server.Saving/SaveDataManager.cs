using PMDDesktop.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Holds methods and fields related to SaveData management. It lives in its own static class to declutter the primary SaveData class.
/// </summary>
public class SaveDataManager : ISaveDataIndexable
{

	/// <summary>
	/// Creates a SaveDataManager.
	/// Typically, this object is a singleton that'll be passed around.
	/// </summary>
	public SaveDataManager()
	{

	}

	/// <summary>
	/// Does this SaveDataManager actually write to files?
	/// </summary>
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
	/// Internal function for getting SaveData by UID.
	/// </summary>
	/// <param name="GUID">The UID of </param>
	/// <returns></returns>
	internal SaveData? GetByGUID(Guid GUID)
	{

		saveDatas.TryGetValue(GUID, out SaveData? data);

		return data;

	}

	/// <summary>
	/// This function scans the save folder and attempts to load all the SaveData in that folder and create objects for them. It should be called once during initialization, and never again.
	/// </summary>
	private async Task LoadAllSaveData()
	{

		// This should find all classes implementing "SaveData."
		IEnumerable<Type> saveTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => typeof(SaveData).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract);

		Dictionary<string, Type> savePaths = [];

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
	/// Asynchronously saves all unsaved SaveData objects, one-by-one. May take time.
	/// Will also delete any SaveData queued for deletion.
	/// Try not to call while SaveData is being updated; saving won't fail but it might make continuity issues.
	/// A good place to call this is when the GameActions queue is empty.
	/// This can only be called once at a time.
	/// </summary>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">Throws if SaveData isn't initalized or if this function is already running.</exception>
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
	/// Returns the folder that this type would be saved to.
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
	/// Returns the folder that this type would be saved to.
	/// </summary>
	/// <param name="data">The data you're looking for the folder path of.</param>
	/// <returns>The folder path of the type.</returns>
	private string GetDirectoryPath(SaveData data)
	{

		return GetDirectoryPath(data.GetType());

	}

	/// <summary>
	/// Returns the exact path (ends with .JSON) that a data is expecting to be written to.
	/// </summary>
	/// <param name="data">The data you're looking for the file path for.</param>
	/// <returns>The file path of the data.</returns>
	private string GetFilePath(SaveData data)
	{

		return Path.Combine(GetDirectoryPath(data.GetType()), data.GUID + ".json");

	}

	/// <summary>
	/// Serializes the data to JSON writes it to a file. Should be called when flushing changes.
	/// </summary>
	/// <param name="data">The data to save</param>
	/// <returns></returns>
	/// <remarks>If LoadFromFilesAndEnableWriting() was not called, it will instead write to a null.</remarks>
	private async Task SaveDataToFile(SaveData data)
	{

		string dirPath = GetDirectoryPath(data);

		if (!Directory.Exists(dirPath))
			Directory.CreateDirectory(dirPath);

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
	/// <returns></returns>
	private async Task EraseSaveDataFile(SaveData data)
	{

		string path = GetFilePath(data);

		// There's a chance we haven't had the chance to flush/save yet, so the file may not exist.
		if (File.Exists(path))
			File.Delete(path);

	}

	/// <summary>
	/// Begin tracking this object and saving it.
	/// </summary>
	/// <param name="data">The SaveData to begin tracking and saving.</param>
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
	/// Disable this SaveData and queue the delition of the file it is saving to.
	/// The object will NOT be disposed of instantly.
	/// The file SaveData will be removed from the list immediately; but the file won't be deleted until the next "flush" happens.
	/// </summary>
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
	/// Allows you to enumerate through tracked SaveData of a type.
	/// </summary>
	/// <typeparam name="T">The type you'd like to enumerate though.</typeparam>
	/// <returns>An enumerable of only the matching type of active save data.</returns>
	public IEnumerable<T> EnumerateData<T>()
	{

		foreach (SaveData data in saveDatas.Values)
			if (data is T typedData)
				yield return typedData;

	}

}
