using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Assets;

public interface IAssetIndexable
{

	T GetAsset<T>(DataIDPath location) where T : Asset;

	bool TryGetAsset<T>(DataIDPath location, [NotNullWhen(true)] out T? asset) where T : Asset;

}
