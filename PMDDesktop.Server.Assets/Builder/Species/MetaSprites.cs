using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="SpeciesSprites"/> before creating it.
/// </summary>
internal class MetaSprites : MetaVisual
{
	internal override AssetLocation Location => GetVisualAssetLocation("sprites");

	internal override async Task<Asset> CreateAsset()
	{

		SpeciesSprites sprites = new(Location);

		await CreateVisualAssetCommon(sprites);

		return sprites;

	}
}
