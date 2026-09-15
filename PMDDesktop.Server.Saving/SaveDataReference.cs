using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Holds a reference to a <see cref="SaveData"/> that persists between saves and loads.
/// </summary>
/// <typeparam name="T">The type of <see cref="SaveData"/> you're holding a reference to.</typeparam>
/// <remarks>
/// <para>Note that during reserialization, if the <see cref="SaveDataReference{SaveData}"/> variable is readonly, it may run into problems with the <see cref="Guid"/> missing.</para>
/// </remarks>
[JsonConverter(typeof(SaveDataReferenceConverterFactory))]
public sealed class SaveDataReference<T> where T : SaveData
{

	/// <summary>
	/// <para>The <see cref="Guid"/> of the referenced <see cref="SaveData"/>.</para>
	/// <para>Saved and loaded between sessions to retrieve referenced <see cref="SaveData"/> again.</para>
	/// </summary>
	[JsonInclude]
	public Guid GUID { get; private set; }

	/// <summary>
	/// <para>Create a new <see cref="SaveDataReference{SaveData}"/> pointing to <paramref name="initalValue"/>'s <see cref="Guid"/>.</para>
	/// <para>This allows you to find the <see cref="SaveData"/> again after serialization and deserialization.</para>
	/// </summary>
	/// <param name="initalValue">The <see cref="SaveData"/> you'd like to keep a reference to via <see cref="Guid"/>.</param>
	public SaveDataReference(T initalValue)
	{

		GUID = initalValue.GUID;

	}

	/// <summary>
	/// <para>Create a new <see cref="SaveDataReference{SaveData}"/> pointing to <paramref name="directGuid"/></para>
	/// <para>The existance of any <see cref="SaveData"/> with this <see cref="Guid"/> is not checked until <see cref="GetReference(ISaveDataIndexable)"/> or <see cref="TryGetReference(ISaveDataIndexable, out T?)"/> is called.</para>
	/// </summary>
	/// <param name="directGuid">The <see cref="Guid"/> you'd like to point to.</param>
	internal SaveDataReference(Guid directGuid)
	{

		GUID = directGuid;

	}

	/// <summary>
	/// Attempt to retrieve the <see cref="SaveData"/> that the saved <see cref="Guid"/> is pointing to using <paramref name="indexable"/>.
	/// </summary>
	/// <param name="indexable">An object that we can find the <see cref="SaveData"/> using, usually links back to or is an <see cref="SaveDataManager"/>.</param>
	/// <returns>The found <see cref="SaveData"/>, or null if not found.</returns>
	public T? GetReference(ISaveDataIndexable indexable)
	{

		return indexable.GetSave<T>(GUID);

	}

	/// <summary>
	/// Attempt to retrieve the <see cref="SaveData"/> that the saved <see cref="Guid"/> is pointing to using <paramref name="indexable"/>.
	/// </summary>
	/// <param name="indexable">An object that we can find the <see cref="SaveData"/> using, usually links back to or is an <see cref="SaveDataManager"/>.</param>
	/// <param name="saveData">The found SaveData, or null if not found.</param>
	/// <returns>True if the <paramref name="saveData"/> was located and is output, otherwise False if <paramref name="saveData"/> couldn't be found and is null.</returns>
	public bool TryGetReference(ISaveDataIndexable indexable, [NotNullWhen(true)] out T? saveData)
	{

		return indexable.TryGetSave(GUID, out saveData);

	}

}
