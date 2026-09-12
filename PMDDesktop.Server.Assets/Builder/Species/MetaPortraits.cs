using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="SpeciesPortraits"/> before creating it.
/// </summary>
internal class MetaPortraits : MetaVisual
{
	internal override AssetLocation Location => GetVisualAssetLocation("portraits");

	internal override async Task<Asset> CreateAsset()
	{

		SpeciesPortraits portraits = new(Location);

		await CreateVisualAssetCommon(portraits);

		return portraits;

	}
}
