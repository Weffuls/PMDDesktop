namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="Asset"/> before creating it.
/// </summary>
internal abstract class MetaAsset
{

	/// <summary>
	/// The <see cref="AssetLocation"/> that <see cref="CreateAsset()"/> will save to. 
	/// </summary>
	internal abstract AssetLocation Location { get; }

	/// <summary>
	/// Create an <see cref="Asset"/> based on this <see cref="MetaAsset"/>.
	/// </summary>
	/// <returns>The created <see cref="Asset"/>.</returns>
	internal abstract Task<Asset> CreateAsset();

}
