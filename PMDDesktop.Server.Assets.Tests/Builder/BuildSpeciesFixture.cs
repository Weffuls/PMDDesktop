using PMDDesktop.Server.Assets.Builder.BuildSteps;
using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.IO.Compression;
using System.Reflection;

namespace PMDDesktop.Server.Assets.Tests.Builder;

public class BuildSpeciesFixture : IAsyncLifetime
{

	private static readonly string ZIP_LOCATION = "PMDDesktop.Server.Assets.Tests.Builder.mockZip.zip";

	public BuildSpeciesFixture()
	{

		Assembly assembly = typeof(BuildSpeciesTests).Assembly;

		Stream stream = assembly.GetManifestResourceStream(ZIP_LOCATION)
			?? throw new FileNotFoundException($"No resource found at {ZIP_LOCATION}");

		ZipArchive archive = new(stream);

		apiZip = new(archive);
		spriteZip = new(archive);

		assets = [];

	}

	internal PokeApiZip apiZip;
	internal SpriteCollabZip spriteZip;
	internal AssetManager assets;

	public async Task InitializeAsync()
	{
		await BuildSpecies.BuildWithCustomZips(assets, apiZip, spriteZip);
	}

	public async Task DisposeAsync()
	{
		// Not needed.
	}

}
