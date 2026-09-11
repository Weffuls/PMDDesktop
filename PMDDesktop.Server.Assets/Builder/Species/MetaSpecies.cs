using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

internal class MetaSpecies : MetaAsset
{

	internal MetaSpecies(JsonElement pokemonSpeciesRoot, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		SpeciesIndex = pokemonSpeciesRoot.GetProperty("id").GetInt32();
		SoloName = pokemonSpeciesRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"No name found on {pokemonSpeciesRoot}");

		ApiZip = apiZip;
		SpriteZip = spriteZip;

		metaAssets.Add(this);

	}

	internal PokeApiZip ApiZip { get; init; }
	internal SpriteCollabZip SpriteZip { get; init; }
	internal string DataName => $"{FormattedIndex}-{SoloName}";
	internal string SoloName { get; init; }
	internal int SpeciesIndex { get; init; }
	internal string FormattedIndex => $"{SpeciesIndex:0000}";

	internal override AssetLocation Location => new("species", DataName);

	internal readonly List<MetaAsset> metaAssets = [];

	internal override async Task<Asset> CreateAsset()
	{

		return new Data.Species(Location);

	}

}
