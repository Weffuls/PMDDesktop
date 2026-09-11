using PMDDesktop.Server.Assets.Data;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// Holds a form and metadata about it, such as the requirements to change into this form.
/// </summary>
/// <param name="form">The form we're talking about.</param>
internal class MetaForm : MetaAsset, INameMatchable
{

	public MetaForm(MetaForm toClone)
	{

		OriginalNames = [.. toClone.OriginalNames];
		Name = toClone.Name;
		Species = toClone.Species;
		FormRoots = [.. toClone.FormRoots];
		genderAlignment = toClone.genderAlignment;

	}

	public MetaForm(MetaSpecies species, JsonElement pokemonFormRoot)
	{

		string originalName = pokemonFormRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"{pokemonFormRoot} had no name property");

		OriginalNames = [originalName];
		Name = originalName;

		FormRoots = [pokemonFormRoot];
		Species = species;

		genderAlignment = BuildSpeciesUtils.HasGenderName(originalName.Split("-"));

	}

	internal List<JsonElement> FormRoots { get; init; }
	internal List<string> OriginalNames { get; init; }
	internal string Name { get; set; }
	internal MetaSpecies Species { get; init; }

	internal GenderAlignment genderAlignment;

	public IEnumerable<string> GetMatchableParts() => Name.Split('-');

	public bool IsStandaloneForm()
	{

		foreach (JsonElement formRoot in FormRoots)
			if (PokeApiUtils.IsPokemonFormStandalone(formRoot))
				return true;

		return false;

	}

	public bool CouldFormsBeMerged(MetaForm with)
	{

		// Merging with ourself destroys ourself.

		if (with == this)
			return false;

		// Check if visuals match before merging.

		IEnumerable<MetaVisual> myVisuals = GetVisualsPointingAt();
		IEnumerable<MetaVisual> withVisuals = with.GetVisualsPointingAt();
		if (!myVisuals.SequenceEqual(withVisuals))
			return false;

		// If more conditions are ever needed before merging, they should be added here.

		return true;

	}

	public void MergeForm(MetaForm target)
	{

		Species.metaAssets.Remove(target);

		// Drop references to the removed target form.
		foreach (MetaVisual visual in target.GetVisualsPointingAt().ToArray())
			visual.DropUnlinkedForms();

		Name = GetCommonName(target);

		// Add items from lists.
		FormRoots.AddRange(target.FormRoots);
		OriginalNames.AddRange(target.OriginalNames);

		// This helps us link Megas that converge with gender differences.
		if (genderAlignment != target.genderAlignment)
			genderAlignment = GenderAlignment.None;

	}

	private string GetCommonName(MetaForm with)
	{

		string? newName = null;

		List<string> withParts = [.. with.Name.Split("-")];

		foreach (string myPart in Name.Split("-"))
		{

			if (withParts.Remove(myPart))
			{
				if (string.IsNullOrWhiteSpace(newName))
					newName = myPart;
				else
					newName += "-" + myPart;
			}

		}

		if (newName == null)
			throw new InvalidOperationException($"Greatest common name between '{Name}' and '{with.Name}' was null.");

		return newName;

	}

	public IEnumerable<MetaVisual> GetVisualsPointingAt() => Species.metaAssets.OfType<MetaVisual>().Where(visual => visual.ForForms.Contains(this));

	internal override AssetLocation Location => new(Species.Location, "forms", Name);

	internal override async Task<Asset> CreateAsset()
	{

		// TODO: Verify if this is the best way to get the "main" form root.
		JsonElement pokemonRoot = await PokeApiUtils.GetPokemonFromForm(FormRoots[0], Species.ApiZip);

		return new SpeciesForm(Location)
		{
			Types = PokeApiUtils.CreateFormTypeReferences(pokemonRoot),
			Stats = PokeApiUtils.CreatePokemonBattleStats(pokemonRoot)
		};

	}

}
