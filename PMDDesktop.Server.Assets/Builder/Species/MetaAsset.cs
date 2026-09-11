namespace PMDDesktop.Server.Assets.Builder.Species;

internal abstract class MetaAsset
{

	internal abstract AssetLocation Location { get; }
	internal abstract Task<Asset> CreateAsset();

}
