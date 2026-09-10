using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Interface for objects that can fetch save data from a save data manager.
/// </summary>
public interface ISaveDataIndexable
{

	/// <summary>
	/// Get a SaveData by type and GUID. Might be null if none is found.
	/// </summary>
	/// <typeparam name="T">The type to get.</typeparam>
	/// <param name="GUID">The GUID of the object you're looking for.</param>
	/// <returns>Will return the object if found, otherwise will return null.</returns>
	T? GetSave<T>(Guid GUID) where T : SaveData;

	/// <summary>
	/// Try to get a save data by type and GUID. Will return false and data will be null if not found.
	/// </summary>
	/// <typeparam name="T">The type of the save data to get.</typeparam>
	/// <param name="GUID">The GUID of the object you're looking for.</param>
	/// <param name="data">The found save data, if any was found. Will be null if the return was false.</param>
	/// <returns>True if the data is found.</returns>
	bool TryGetSave<T>(Guid GUID, [NotNullWhen(true)] out T? data) where T : SaveData;

}
