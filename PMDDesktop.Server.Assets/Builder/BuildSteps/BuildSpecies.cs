using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.BuildSteps;

internal static class BuildSpecies
{

	public static async Task StartBuildStep(AssetManager assets)
	{

		await BuildTopLevelSpecies();

		return;

	}

	private static async Task BuildTopLevelSpecies()
	{

		using PokeApiZip zip = await ZipManager.GetPokeApiZip();

		foreach (ZipArchiveEntry entry in zip.EnumerateSpecies())
		{

			using Stream stream = await entry.OpenAsync();

			JsonDocument json = await JsonDocument.ParseAsync(stream);

			int speciesNumber = json.RootElement.GetProperty("id").GetInt32();
			string speciesName = json.RootElement.GetProperty("name").GetString()
				?? throw new Exception($"No name found on {json}");

			AssetLocation location = new("species", $"{speciesNumber:0000}-{speciesName}");

			Species species = new(location);

			throw new NotImplementedException();

		}

	}

}
