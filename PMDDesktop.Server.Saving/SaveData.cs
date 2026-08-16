using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// SaveData is an abstract class that implements features to help with creating persistant data.
///
/// Each SaveData has a GUID that identifies it.
///
/// Pass the SaveData into the SaveDataManager's Add() function to start saving it, or Delete() to stop saving it.
/// </summary>
public abstract class SaveData
{

	/// <summary>
	/// Was this SaveData edited and has unsaved changes?
	/// Control this property with MarkDirty() and Save().
	/// Marking a SaveData dirty may allow it to be "autosaved" by other functions, for example on program quit.
	/// </summary>
	[JsonIgnore]
	public bool Dirty { get; internal set; } = false;

	/// <summary>
	/// The unique identifier for this object.
	/// This UID will be used to name the save file that is written to.
	/// Checks are performed to ensure this UID is unique.
	/// Creating an object with a matching UID will throw.
	/// </summary>
	[JsonIgnore]
	public Guid GUID { get; internal set; }

	/// <summary>
	/// The application version that this SaveData was originally created in.
	/// Used for future-proofing, incase data structure upgrades ever need to be done.
	/// </summary>
	public Version CreationVersion { get; init; }

	/// <summary>
	/// The date and time that this SaveData was originally created in.
	/// This is not reliable for upgrading data between versions, as servers could be running an older version, but it is a cool statistic, and may be useful for debugging.
	/// </summary>
	public DateTime CreationDate { get; init; }

	/// <summary>
	/// The SaveDataManager this SaveData belongs to. Can be null if not yet assigned to a SaveDataManager.
	/// </summary>
	/// <remarks>
	/// To avoid entering an invalid state, do not interchange SaveDatas between SaveDataManagers.
	/// </remarks>
	[JsonIgnore]
	public SaveDataManager? Manager { get; internal set; }

	/// <summary>
	/// Creates a new Save Data instance. The GUID will be a randomly generated GUID.
	/// Immediately marked Dirty, add it to the SaveDataManager to start saving this data.
	/// </summary>
	protected SaveData()
	{

		GUID = Guid.NewGuid();

		CreationVersion = AppInfo.VERSION;
		CreationDate = DateTime.Now;

		Dirty = true;

	}

	/// <summary>
	/// Marks the object as dirty (unsaved) saying it needs to be saved.
	/// </summary>
	/// <remarks>As a design practice, this should always be called by the function that's making the changes to the object, and never by the object itself.</remarks>
	public void MarkDirty()
	{

		Dirty = true;

	}

	public override string ToString()
	{
		return $"{GetType().Name}[{GUID}]";
	}

	/// <summary>
	/// This can be overridden if you'd like to make last second changes to an object before saving.
	/// </summary>
	protected internal virtual void OnBeforeSave() { }

	/// <summary>
	/// <para>
	/// This can be overridden if you'd like to run code when a new save data is removed from the save data manager.
	/// </para>
	/// <para>
	///	If you had references to that SaveData and it's being removed, now would be a good time to delete them.
	/// </para>
	/// </summary>
	/// <remarks>Note that this can be called on itself AND that it will only be called when this SaveData is managed by the SaveDataManager.</remarks>
	/// <param name="data"></param>
	protected internal virtual void OnAnySaveDataRemoved(SaveData data) { }

}
