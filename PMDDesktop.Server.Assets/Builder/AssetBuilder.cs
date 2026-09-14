using PMDDesktop.Server.Assets.Builder.Species;
using PMDDesktop.Server.Assets.Builder.Types;
using PMDDesktop.Structs;

namespace PMDDesktop.Server.Assets.Builder;

public static class AssetBuilder
{

	public static readonly BuildStep[] BUILD_STEPS =
	[
		new("types", BuildTypes.StartBuildStep),
		new("species", BuildSpecies.StartBuildStep)
	];

	public static async Task<int> RunAllSteps(bool writeAssets = true)
	{

		AssetManager assets = [];

		foreach (BuildStep step in BUILD_STEPS)
		{
			await step.onExecute(assets);
		}

		if (writeAssets)
			await assets.WriteAllAssets();

		return 0;

	}

	public static void DeleteAssetsFolder()
	{

		if (Directory.Exists(AssetLocation.GetAssetsDirectory()))
			Directory.Delete(AssetLocation.GetAssetsDirectory(), true);

	}

}
