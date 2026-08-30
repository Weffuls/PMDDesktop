using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.BuildSteps;

internal class BuildTypes
{

	public static async Task StartBuildStep(AssetManager assets)
	{

		await BuildTypeAssets(assets);

		return;

	}

	private static async Task BuildTypeAssets(AssetManager assets)
	{

		using PokeApiZip zip = await ZipManager.GetPokeApiZip();

		foreach (ZipArchiveEntry entry in zip.EnumerateTypes())
		{

			using Stream stream = await entry.OpenAsync();

			using JsonDocument json = JsonDocument.Parse(stream);
			JsonElement root = json.RootElement;

			string typeName = json.RootElement.GetProperty("name").GetString()
				?? throw new Exception();

			PokemonType type = new(PokemonType.DefaultTypeLocation(typeName));

			JsonElement damageRelations = root.GetProperty("damage_relations");

			type.Resistances = CreateTypeReferences(damageRelations.GetProperty("half_damage_from"));
			type.Weaknesses = CreateTypeReferences(damageRelations.GetProperty("double_damage_from"));
			type.Immunities = CreateTypeReferences(damageRelations.GetProperty("no_damage_from"));

			assets.Add(type);

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

}
