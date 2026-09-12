using PMDDesktop.Server.Assets.Data;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="SpeciesForm"/> before creating it.
/// </summary>
internal class MetaForm : MetaAsset, INameMatchable
{

	/// <summary>
	/// Create a new <see cref="MetaForm"/> by cloning <paramref name="toClone"/>.
	/// </summary>
	/// <param name="toClone">The <see cref="MetaForm"/> to clone.</param>
	public MetaForm(MetaForm toClone)
	{

		OriginalNames = [.. toClone.OriginalNames];
		Name = toClone.Name;
		Species = toClone.Species;
		FormRoots = [.. toClone.FormRoots];
		genderAlignment = toClone.genderAlignment;

	}

	/// <summary>
	/// Create a new <see cref="MetaForm"/> based on <paramref name="pokemonFormRoot"/>.
	/// </summary>
	/// <param name="species">The <see cref="MetaSpecies"/> this <see cref="MetaForm"/> belongs to.</param>
	/// <param name="pokemonFormRoot">The root of a "pokemon-form" JSON object to base this <see cref="MetaForm"/> on.</param>
	/// <exception cref="InvalidDataException"></exception>
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

	/// <summary>
	/// All "pokemon-form" objects that this form consists of.
	/// </summary>
	internal List<JsonElement> FormRoots { get; init; }

	/// <summary>
	/// The original names of all "pokemon-form" objects that this form consists of.
	/// </summary>
	internal List<string> OriginalNames { get; init; }

	/// <summary>
	/// The name of this <see cref="MetaForm"/>. Also used to determine the <see cref="Location"/>.
	/// </summary>
	internal string Name { get; set; }

	/// <summary>
	/// The species that this <see cref="MetaForm"/> belongs to.
	/// </summary>
	internal MetaSpecies Species { get; init; }

	/// <summary>
	/// Does this <see cref="MetaForm"/> align with a gender? If it does, only <see cref="MetaVariant"/>s of a connectable gender should link to this <see cref="MetaForm"/>.
	/// </summary>
	internal GenderAlignment genderAlignment;

	public IEnumerable<string> GetMatchableParts() => Name.Split('-');

	/// <summary>
	/// <para>Opinionated: Is this form "standalone?"</para>
	/// <para>This is intended for use to find out which <see cref="MetaForm"/>s should be the base form for <see cref="MetaVariant"/>s.</para>
	/// </summary>
	/// <returns>True if it is "standalone," otherwise false.</returns>
	public bool IsStandaloneForm()
	{

		foreach (JsonElement formRoot in FormRoots)
			if (PokeApiUtils.IsPokemonFormStandalone(formRoot))
				return true;

		return false;

	}

	/// <summary>
	/// <para>Do the visuals found by <see cref="GetClosestVisualMatches()"/> suggest that this <see cref="MetaForm"/> would need to be split into both a <see cref="GenderAlignment.Female"/> <see cref="MetaForm"/> and a <see cref="GenderAlignment.Male"/> <see cref="MetaForm"/> to properly represent gender differences?</para>
	/// <para>In other words, does this form see both <see cref="GenderAlignment.Male"/> and <see cref="GenderAlignment.Female"/> visuals?</para>
	/// </summary>
	/// <returns>True if this <see cref="MetaForm"/> should be split, false otherwise.</returns>
	internal async Task<bool> NeedsGenderSplit()
	{

		if (genderAlignment != GenderAlignment.None)
			return false;

		IEnumerable<MetaVisual> visuals = await GetClosestVisualMatches();

		bool seenMale = false;
		bool seenFemale = false;

		foreach (MetaVisual visual in visuals)
		{

			if (visual.genderAlignment == GenderAlignment.Male)
				seenMale = true;

			if (visual.genderAlignment == GenderAlignment.Female)
				seenFemale = true;

		}

		return seenMale && seenFemale;

	}

	/// <summary>
	/// <para>Highly opinionated function to attempt to match <see cref="MetaVisual"/>s with the this <see cref="MetaForm"/></para>
	/// </summary>
	/// <returns>An enumerable of <see cref="MetaVisual"/>s that likely match this <see cref="MetaForm"/>.</returns>
	/// <remarks>
	/// <para>Improvements to the matching algorithm are welcome.</para>
	/// </remarks>
	internal async Task<IEnumerable<MetaVisual>> GetClosestVisualMatches()
	{

		int highestMatchResult = int.MinValue; // Matches
		int lowestSpecificityTiebreaker = int.MaxValue; // Tie-breaker, so lower counts with the same match count are prioritized.
		List<MetaVisual> foundVisuals = [];

		foreach (MetaVisual visual in Species.metaAssets.OfType<MetaVisual>())
		{

			if (!BuildSpeciesUtils.IsConnectableGender(genderAlignment, visual.genderAlignment))
				continue;

			int specificity = visual.GetMatchableParts().Count();
			int matchResults = INameMatchable.CalculateNameMatches(this, visual);

			// If this is not the best match, leave.
			if (matchResults < highestMatchResult)
				continue;

			// If this is tied for the best match, but more specific, leave.
			if (matchResults == highestMatchResult && specificity > lowestSpecificityTiebreaker)
				continue;

			// If this is the new best match (not a tie), wipe the list.
			if (matchResults > highestMatchResult)
				foundVisuals.Clear();

			foundVisuals.Add(visual);
			highestMatchResult = matchResults;
			lowestSpecificityTiebreaker = specificity;
			continue;

		}

		return foundVisuals;

	}

	/// <summary>
	/// Opinionated: Could <paramref name="mergeTarget"/> merge into this <see cref="MetaForm"/>?
	/// </summary>
	/// <param name="mergeTarget">The <see cref="MetaForm"/> that will be checking its ability to merge into this <see cref="MetaForm"/>.</param>
	/// <returns>True if <paramref name="mergeTarget"/> could merge into this <see cref="MetaForm"/>.</returns>
	public bool CouldFormsBeMerged(MetaForm mergeTarget)
	{

		// Merging with ourself destroys ourself.

		if (mergeTarget == this)
			return false;

		// Check if visuals match before merging.

		IEnumerable<MetaVisual> myVisuals = GetVisualsPointingAt();
		IEnumerable<MetaVisual> withVisuals = mergeTarget.GetVisualsPointingAt();
		if (!myVisuals.SequenceEqual(withVisuals))
			return false;

		// If more conditions are ever needed before merging, they should be added here.

		return true;

	}

	/// <summary>
	/// <para>Merge <paramref name="mergeTarget"/> into this <see cref="MetaForm"/>.</para>
	/// <para>This will remove <paramref name="mergeTarget"/> from the <see cref="MetaSpecies.metaAssets"/> list.</para>
	/// </summary>
	/// <param name="mergeTarget">The <see cref="MetaForm"/> that will be merged into this <see cref="MetaForm"/>.</param>
	public void MergeForm(MetaForm mergeTarget)
	{

		Species.metaAssets.Remove(mergeTarget);

		// Drop references to the removed target form.
		foreach (MetaVisual visual in mergeTarget.GetVisualsPointingAt().ToArray())
			visual.DropUnreferencedForms();

		Name = GetCommonName(mergeTarget);

		// Add items from lists.
		FormRoots.AddRange(mergeTarget.FormRoots);
		OriginalNames.AddRange(mergeTarget.OriginalNames);

		// This helps us link Megas that converge with gender differences.
		if (genderAlignment != mergeTarget.genderAlignment)
			genderAlignment = GenderAlignment.None;

	}

	/// <summary>
	/// Combine the names of two <see cref="MetaForm"/>s to create a common name between them.
	/// </summary>
	/// <param name="with">The other <see cref="MetaForm"/> to combine with.</param>
	/// <returns>A string consisting of a new name made up of only the common parts.</returns>
	/// <exception cref="InvalidOperationException">If both names had nothing in common.</exception>
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

	/// <summary>
	/// Get a list of all <see cref="MetaVisual"/>s assigned to this <see cref="MetaForm"/> inside the assigned <see cref="MetaSpecies"/>.
	/// </summary>
	/// <returns>An enumerable with all <see cref="MetaVisual"/>s pointing at this <see cref="MetaForm"/>.</returns>
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
