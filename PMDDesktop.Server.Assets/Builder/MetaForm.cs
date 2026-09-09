using PMDDesktop.Server.Assets.Data;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// Holds a form and metadata about it, such as the requirements to change into this form.
/// </summary>
/// <param name="form">The form we're talking about.</param>
internal class MetaForm : MetaAsset, INameMatchable
{

	public MetaForm(MetaSpecies species, JsonElement pokemonFormRoot)
	{

		OriginalName = pokemonFormRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"{pokemonFormRoot} had no name property");

		Name = OriginalName;

		FormRoot = pokemonFormRoot;
		Species = species;

	}

	internal JsonElement FormRoot { get; init; }
	internal string OriginalName { get; init; }
	internal string Name { get; set; }
	internal MetaSpecies Species { get; init; }

	internal GenderAlignment genderAlignment = GenderAlignment.None;

	public IEnumerable<string> GetMatchableParts() => OriginalName.Split('-');

	public bool IsStandaloneForm()
	{

		return PokeApiUtils.IsPokemonFormStandalone(FormRoot);

	}

	internal override AssetLocation Location => new(Species.Location, "forms", Name);

	internal override async Task<Asset> CreateAsset()
	{

		JsonElement pokemonRoot = await PokeApiUtils.GetPokemonFromForm(FormRoot, Species.ApiZip);

		return new SpeciesForm(Location)
		{
			Types = PokeApiUtils.CreateFormTypeReferences(FormRoot),
			Stats = PokeApiUtils.CreatePokemonBattleStats(pokemonRoot)
		};

	}

}
