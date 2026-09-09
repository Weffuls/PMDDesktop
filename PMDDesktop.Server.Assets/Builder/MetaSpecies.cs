using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

internal class MetaSpecies : MetaAsset
{

	internal MetaSpecies(JsonElement pokemonSpeciesRoot, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		int speciesNumber = pokemonSpeciesRoot.GetProperty("id").GetInt32();
		string speciesName = pokemonSpeciesRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"No name found on {pokemonSpeciesRoot}");

		DataName = $"{speciesNumber:0000}-{speciesName}";

		ApiZip = apiZip;
		SpriteZip = spriteZip;

		metaAssets.Add(this);

	}

	internal PokeApiZip ApiZip { get; init; }
	internal SpriteCollabZip SpriteZip { get; init; }
	internal string DataName { get; init; }

	internal override AssetLocation Location => new("species", DataName);

	internal readonly List<MetaAsset> metaAssets = [];

	internal override async Task<Asset> CreateAsset()
	{

		return new Species(Location);

	}

}
