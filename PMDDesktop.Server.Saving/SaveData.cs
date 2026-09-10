using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// <para>SaveData is an abstract class that implements features to help with creating persistant data.</para>
/// <para>Each SaveData has a GUID that identifies it.</para>
/// <para>Pass the SaveData into the SaveDataManager's Add() function to start saving it, or Delete() to stop saving it.</para>
/// </summary>
public abstract class SaveData
{

	/// <summary>
	/// <para>Was this SaveData edited and has unsaved changes?</para>
	/// <para>Control this property with <b>MarkDirty()</b>.</para>
	/// <para>Marking a SaveData dirty allows it to be saved by the SaveManager, for example on program quit.</para>
	/// </summary>
	[JsonIgnore]
	public bool Dirty { get; internal set; } = false;

	/// <summary>
	/// <para>The unique identifier for this object.</para>
	/// <para>This GUID will be used to name the save file that is written to.</para>
	/// <para>Checks are performed to ensure this GUID is unique.</para>
	/// </summary>
	[JsonIgnore]
	public Guid GUID { get; internal set; }

	/// <summary>
	/// <para>The application version that this SaveData was originally created in.</para>
	/// <para>Used for future-proofing, incase data structure upgrades ever need to be done.</para>
	/// </summary>
	public Version CreationVersion { get; init; }

	/// <summary>
	/// <para>The date and time that this SaveData was originally created in.</para>
	/// <para>This is not reliable for upgrading data between versions, as servers could be running an older version, but it is a cool statistic, and may be useful for debugging.</para>
	/// </summary>
	public DateTime CreationDate { get; init; }

	/// <summary>
	/// <para>Creates a new Save Data instance. The GUID will be a randomly generated GUID.</para>
	/// <para>It will initally be marked Dirty; add it to the SaveDataManager to start saving this data.</para>
	/// </summary>
	protected SaveData()
	{

		GUID = Guid.NewGuid();

		CreationVersion = AppInfo.VERSION;
		CreationDate = DateTime.Now;

		Dirty = true;

	}

	/// <summary>
	/// Marks the object as dirty (unsaved), saying it needs to be saved.
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
