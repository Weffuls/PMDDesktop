using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Server.Assets;

public interface IAssetIndexable
{

	T GetAsset<T>(AssetLocation location) where T : Asset;

	bool TryGetAsset<T>(AssetLocation location, [NotNullWhen(true)] out T? asset) where T : Asset;

}
