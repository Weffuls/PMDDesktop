using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Holds a reference to a SaveData that persists between saves and loads.
/// </summary>
/// <typeparam name="T">The type of SaveData you're holding a reference to.</typeparam>
/// <remarks>
/// Not caching results here to keep garbage collection intact. Optimization or caching ideas are welcome.
/// Note that during reserialization, if the SaveDataReference is readonly, it may run into problems with the GUID missing.
/// </remarks>
[JsonConverter(typeof(SaveDataReferenceConverter<>))]
public sealed class SaveDataReference<T> where T : SaveData
{

	[JsonInclude]
	public Guid GUID { get; private set; }

	public SaveDataReference(T initalValue)
	{

		GUID = initalValue.GUID;

	}

	internal SaveDataReference(Guid directGuid)
	{

		GUID = directGuid;

	}

	public T? GetReference(ISaveDataIndexable indexable)
	{

		return indexable.GetSave<T>(GUID);

	}

	public bool TryGetReference(ISaveDataIndexable indexable, [NotNullWhen(true)] out T? saveData)
	{

		return indexable.TryGetSave(GUID, out saveData);

	}

}
