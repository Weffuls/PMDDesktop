using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// Interface for objects that can fetch <see cref="SaveData"/> from a <see cref="SaveDataManager"/>.
/// </summary>
public interface ISaveDataIndexable
{

	/// <summary>
	/// Get a <see cref="SaveData"/> by type and <see cref="Guid"/>. Might be null if none is found.
	/// </summary>
	/// <typeparam name="T">The type to get.</typeparam>
	/// <param name="GUID">The <see cref="Guid"/> of the <see cref="SaveData"/> you're looking for.</param>
	/// <returns>Will return the <see cref="SaveData"/> if found, otherwise will return null.</returns>
	T? GetSave<T>(Guid GUID) where T : SaveData;

	/// <summary>
	/// Try to get a <see cref="SaveData"/> by type and <see cref="Guid"/>. Will return false and <paramref name="data"/> will be null if not found.
	/// </summary>
	/// <typeparam name="T">The type of the <see cref="SaveData"/> to get.</typeparam>
	/// <param name="GUID">The <see cref="Guid"/> of the <see cref="SaveData"/> you're looking for.</param>
	/// <param name="data">The found <see cref="SaveData"/>, if any was found. Will be null if the return was false.</param>
	/// <returns>True if the <see cref="SaveData"/> is found.</returns>
	bool TryGetSave<T>(Guid GUID, [NotNullWhen(true)] out T? data) where T : SaveData;

}
