using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.BuildSteps;

internal static class BuildSpecies
{

	private class SpeciesRecord
	{

		public string identifier = string.Empty;

	}

	public static async Task StartBuildStep()
	{

		await BuildTypes();

		await BuildTopLevelSpecies();

		return;

	}

	private static async Task BuildTypes()
	{

		using PokeApiZip zip = await ZipManager.GetPokeApiZip();

		foreach (ZipArchiveEntry entry in zip.EnumerateTypes())
		{

			using Stream stream = await entry.OpenAsync();

			JsonDocument json = JsonDocument.Parse(stream);
			JsonElement root = json.RootElement;

			string typeName = json.RootElement.GetProperty("name").GetString()
				?? throw new Exception();

			PokemonType type = new(PokemonType.DefaultTypeLocation(typeName));

			JsonElement damageRelations = root.GetProperty("damage_relations");

			type.Resistances = CreateTypeReferences(damageRelations.GetProperty("half_damage_from"));
			type.Weaknesses = CreateTypeReferences(damageRelations.GetProperty("double_damage_from"));
			type.Immunities = CreateTypeReferences(damageRelations.GetProperty("no_damage_from"));

			await AssetManager.WriteAsset(type);

		}

	}

	private static ImmutableArray<AssetReference<PokemonType>> CreateTypeReferences(JsonElement element)
	{

		ImmutableArray<AssetReference<PokemonType>>.Builder list = ImmutableArray.CreateBuilder<AssetReference<PokemonType>>(element.GetArrayLength());

		foreach (JsonElement type in element.EnumerateArray())
		{

			string typeName = type.GetProperty("name").GetString()
				?? throw new InvalidDataException($"Read string \"name\" from {type} was null.");

			list.Add(new(PokemonType.DefaultTypeLocation(typeName)));

		}

		return list.ToImmutable();

	}

	private static async Task BuildTopLevelSpecies()
	{

		using PokeApiZip zip = await ZipManager.GetPokeApiZip();

		foreach (ZipArchiveEntry entry in zip.EnumerateSpecies())
		{

			using Stream stream = await entry.OpenAsync();

			JsonDocument json = JsonDocument.Parse(stream);

			int speciesNumber = json.RootElement.GetProperty("id").GetInt32();
			string speciesName = json.RootElement.GetProperty("name").GetString()
				?? throw new Exception();

			AssetLocation location = new("species", $"{speciesNumber:0000}-{speciesName}");

			Species species = new(location);

			await AssetManager.WriteAsset(species);

		}

	}

}
