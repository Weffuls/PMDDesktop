using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// <para><see cref="SaveData"/> is an abstract class that implements features to help with creating persistant data.</para>
/// <para>Each <see cref="SaveData"/> has a <see cref="Guid"/> that identifies it.</para>
/// <para>Pass the <see cref="SaveData"/> into the <see cref="SaveDataManager.Add(PMDDesktop.Server.Saving.SaveData)"/> function to start saving it, or <see cref="SaveDataManager.Remove(PMDDesktop.Server.Saving.SaveData)"/> to stop saving it.</para>
/// </summary>
public abstract class SaveData
{

	/// <summary>
	/// <para>Was this <see cref="SaveData"/> edited and has unsaved changes?</para>
	/// <para>Control this property with <see cref="MarkDirty()"/>.</para>
	/// <para>Marking a <see cref="SaveData"/> dirty allows it to be saved by the <see cref="SaveDataManager"/> when <see cref="SaveDataManager.SaveAllChanges()"/> is called.</para>
	/// </summary>
	[JsonIgnore]
	public bool Dirty { get; internal set; } = false;

	/// <summary>
	/// <para>The unique identifier for this <see cref="SaveData"/>.</para>
	/// <para>This <see cref="Guid"/> will be used to name the save file that is written to.</para>
	/// <para>Checks are performed by the <see cref="SaveDataManager"/> to ensure this <see cref="Guid"/> is unique.</para>
	/// </summary>
	[JsonIgnore]
	public Guid GUID { get; internal set; }

	/// <summary>
	/// <para>The application version that this <see cref="SaveData"/> was originally created in.</para>
	/// <para>Used for future-proofing, incase data structure upgrades ever need to be done.</para>
	/// </summary>
	public Version CreationVersion { get; init; }

	/// <summary>
	/// <para>The date and time that this <see cref="SaveData"/> was originally created in.</para>
	/// <para>This is not reliable for upgrading data between versions, as servers could be running an older version, but it is a cool statistic, and may be useful for debugging.</para>
	/// </summary>
	public DateTime CreationDate { get; init; }

	/// <summary>
	/// <para>Creates a new <see cref="SaveData"/> instance. The <see cref="Guid"/> will be a randomly generated <see cref="Guid"/>.</para>
	/// <para><see cref="Dirty"/> will initally be true; add it to the <see cref="SaveDataManager"/> to start saving this data.</para>
	/// </summary>
	protected internal SaveData()
	{

		GUID = Guid.NewGuid();

		CreationVersion = AppInfo.VERSION;
		CreationDate = DateTime.Now;

		Dirty = true;

	}

	/// <summary>
	/// <para>Marks the object as dirty (unsaved), saying it needs to be saved.</para>
	/// <para>That is, sets <see cref="Dirty"/> to true.</para>
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
	/// This can be overridden if you'd like to make last second changes to a <see cref="SaveData"/> object before saving.
	/// </summary>
	protected internal virtual void OnBeforeSave() { }

	/// <summary>
	/// <para>
	/// This can be overridden if you'd like to run code when a new <see cref="SaveData"/> is removed from the <see cref="SaveDataManager"/>.
	/// </para>
	/// <para>
	///	If you had references to that <see cref="SaveData"/> and it's being removed, now would be a good time to delete them.
	/// </para>
	/// </summary>
	/// <remarks>Note that this can be called on itself AND that it will only be called when this <see cref="SaveData"/> is managed by a <see cref="SaveDataManager"/>.</remarks>
	/// <param name="data"></param>
	protected internal virtual void OnAnySaveDataRemoved(SaveData data) { }

}
