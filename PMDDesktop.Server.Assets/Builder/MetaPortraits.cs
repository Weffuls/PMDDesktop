using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder;

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
