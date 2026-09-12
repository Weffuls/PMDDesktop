using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="Data.Species"/> before creating it.
/// </summary>
internal class MetaSpecies : MetaAsset
{

	/// <summary>
	/// Create a new <see cref="MetaSpecies"/> based on a <paramref name="pokemonSpeciesRoot"/>.
	/// </summary>
	/// <param name="pokemonSpeciesRoot">The root of a "pokemon-species" JSON object to base this <see cref="MetaSpecies"/> on.</param>
	/// <param name="apiZip"><see cref="PokeApiZip"/> to use when working with this <see cref="MetaSpecies"/>.</param>
	/// <param name="spriteZip"><see cref="SpriteCollabZip"/> to use when working with this <see cref="MetaSpecies"/>.</param>
	/// <exception cref="InvalidDataException"></exception>
	internal MetaSpecies(JsonElement pokemonSpeciesRoot, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		SpeciesIndex = pokemonSpeciesRoot.GetProperty("id").GetInt32();
		SoloName = pokemonSpeciesRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"No name found on {pokemonSpeciesRoot}");

		ApiZip = apiZip;
		SpriteZip = spriteZip;

		metaAssets.Add(this);

	}

	/// <summary>
	/// Quick reference to the <see cref="PokeApiZip"/> for when working with this type.
	/// </summary>
	internal PokeApiZip ApiZip { get; init; }

	/// <summary>
	/// Quick reference to the <see cref="PokeApiZip"/> for when working with this type.
	/// </summary>
	internal SpriteCollabZip SpriteZip { get; init; }

	/// <summary>
	/// The name of the tail end of the <see cref="Location"/> that the <see cref="Data.Species"/> will be saved to.
	/// </summary>
	internal string DataName => $"{FormattedIndex}-{SoloName}";

	/// <summary>
	/// The name of the species alone, (e.g. "pikachu" or "mudkip")
	/// </summary>
	internal string SoloName { get; init; }

	/// <summary>
	/// The Pokédex Number of the species.
	/// </summary>
	internal int SpeciesIndex { get; init; }

	/// <summary>
	/// The Pokédex Number of the species, but padded to 4 digits.
	/// </summary>
	internal string FormattedIndex => $"{SpeciesIndex:0000}";

	internal override AssetLocation Location => new("species", DataName);

	/// <summary>
	/// A list of all <see cref="MetaAsset"/>s connected to this <see cref="MetaSpecies"/>, <b>including itself</b>.
	/// </summary>
	internal readonly List<MetaAsset> metaAssets = [];

	internal override async Task<Asset> CreateAsset()
	{

		return new Data.Species(Location);

	}

}
