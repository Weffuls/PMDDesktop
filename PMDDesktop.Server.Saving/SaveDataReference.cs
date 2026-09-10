using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Holds a reference to a SaveData that persists between saves and loads.
/// </summary>
/// <typeparam name="T">The type of SaveData you're holding a reference to.</typeparam>
/// <remarks>
/// <para>Note that during reserialization, if the SaveDataReference variable is readonly, it may run into problems with the GUID missing.</para>
/// </remarks>
[JsonConverter(typeof(SaveDataReferenceConverter<>))]
public sealed class SaveDataReference<T> where T : SaveData
{

	/// <summary>
	/// <para>The GUID of the referenced SaveData.</para>
	/// <para>Saved and loaded between sessions to retrieve referenced SaveData again.</para>
	/// </summary>
	[JsonInclude]
	public Guid GUID { get; private set; }

	/// <summary>
	/// <para>Create a new SaveDataReference pointing to <paramref name="initalValue"/>'s GUID.</para>
	/// <para>This allows you to find the SaveData again after serialization and deserialization.</para>
	/// </summary>
	/// <param name="initalValue">The SaveData you'd like to keep a reference to via GUID.</param>
	public SaveDataReference(T initalValue)
	{

		GUID = initalValue.GUID;

	}

	/// <summary>
	/// <para>Create a new SaveDataReference pointing to <paramref name="directGuid"/></para>
	/// <para>The existance of any SaveData with this GUID is not checked until GetReference() is called.</para>
	/// </summary>
	/// <param name="directGuid">The GUID you'd like to point to.</param>
	internal SaveDataReference(Guid directGuid)
	{

		GUID = directGuid;

	}

	/// <summary>
	/// Attempt to retrieve the SaveData that the saved GUID is pointing to using <paramref name="indexable"/>.
	/// </summary>
	/// <param name="indexable">An object that we can find the SaveData using, usually links back to or is an SaveManager.</param>
	/// <returns>The found SaveData, or null if not found.</returns>
	public T? GetReference(ISaveDataIndexable indexable)
	{

		return indexable.GetSave<T>(GUID);

	}

	/// <summary>
	/// Attempt to retrieve the SaveData that the saved GUID is pointing to using <paramref name="indexable"/>.
	/// </summary>
	/// <param name="indexable">An object that we can find the SaveData using, usually links back to or is an SaveManager.</param>
	/// <param name="saveData">The found SaveData, or null if not found.</param>
	/// <returns>True if the <paramref name="saveData"/> was located and is output, otherwise False if <paramref name="saveData"/> couldn't be found and is null.</returns>
	public bool TryGetReference(ISaveDataIndexable indexable, [NotNullWhen(true)] out T? saveData)
	{

		return indexable.TryGetSave(GUID, out saveData);

	}

}
