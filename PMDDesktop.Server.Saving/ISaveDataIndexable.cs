using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Saving;

public interface ISaveDataIndexable
{

	T? GetSave<T>(Guid GUID) where T : SaveData;

	bool TryGetSave<T>(Guid GUID, [NotNullWhen(true)] out T? data) where T : SaveData;

}
