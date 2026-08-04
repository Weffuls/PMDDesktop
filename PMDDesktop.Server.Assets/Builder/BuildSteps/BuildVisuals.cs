using PMDDesktop.Server.Assets.Builder.ZipScavenger;

namespace PMDDesktop.Server.Assets.Builder.BuildSteps;

internal static class BuildVisuals
{

	public static async Task StartBuildStep()
	{

		using SpriteCollabZip zip = await ZipManager.GetSpriteCollabZip();

		return;

	}

}
