using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="SpeciesSprites"/> before creating it.
/// </summary>
internal class MetaSprites : MetaVisual
{
	internal override DataIDPath Path => GetVisualAssetLocation("sprites");

	internal override async Task<Asset> CreateAsset()
	{

		SpeciesSprites sprites = new(Path);

		await CreateVisualAssetCommon(sprites);

		return sprites;

	}
}
