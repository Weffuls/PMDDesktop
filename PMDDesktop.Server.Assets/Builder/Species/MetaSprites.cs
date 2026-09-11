using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder.Species;

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
