using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// This file contains extremely specialized helper functions for BuildSpecies relating to the PMDCollab's SpriteCollab.
/// They are delegated here to help BuildSpecies be easy to follow the flow of.
/// </summary>
internal static class SpriteCollabUtils
{

	/// <summary>
	/// Opinionated list of groups to skip.
	/// These don't make much sense in our program.
	/// </summary>
	public static readonly string[] EXCLUDED_GROUPS = ["cutscene", "skytemple"];

	public static async Task<JsonElement> GetSpeciesTop(string index, SpriteCollabZip zip)
	{

		JsonElement tracker = await zip.GetTrackerJSON();

		return tracker.GetProperty(index);

	}

	public static async Task<IEnumerable<MetaVisual>> GetAllVisuals(MetaSpecies species)
	{

		JsonElement top = await GetSpeciesTop(species.FormattedIndex, species.SpriteZip);

		List<MetaVisual> visuals = [.. await RecurseSubgroups(species, top, [])];

		// Make sure that each male/female has a counterpart.
		// This is run seperately for portraits & sprites so that they do not affect each other.
		// Some species have ungendered sprites but have gendered portraits.
		ReflectVisualGendersInPlace(visuals.OfType<MetaSprites>());
		ReflectVisualGendersInPlace(visuals.OfType<MetaPortraits>());

		return visuals;

	}

	private static void ReflectVisualGendersInPlace(IEnumerable<MetaVisual> visuals)
	{

		// Baked to array so that visuals we flip the gender of in this function don't get re-run.
		MetaVisual[] genderedVisuals = [.. visuals.Where(visual => visual.genderAlignment != GenderAlignment.None)];

		foreach (MetaVisual reflecting in genderedVisuals)
			foreach (MetaVisual target in visuals)
			{

				// Both the target and the reflector need to share shininess.
				// Theoretically, this shouldn't cause any problems, but better safe than sorry.
				if (reflecting.isShiny != target.isShiny)
					continue;

				// It only makes sense to reflect neutral visuals.
				if (target.genderAlignment != GenderAlignment.None)
					continue;

				// This function excludes some terms like 'male' and 'female' so it works for this use case.
				if (!INameMatchable.IsSameNameAnyOrder(reflecting, target))
					continue;

				target.genderAlignment = GetReflectedGender(reflecting.genderAlignment);

			}

	}

	/// <summary>
	/// Returns the "reflected" or "opposite" gender. That is, male into female and female into male.
	/// </summary>
	/// <param name="gender">The gender to reflect.</param>
	/// <returns>The "reflected" or "opposite" gender of the input.</returns>
	private static GenderAlignment GetReflectedGender(GenderAlignment gender)
	{

		if (gender == GenderAlignment.Male)
			return GenderAlignment.Female;

		if (gender == GenderAlignment.Female)
			return GenderAlignment.Male;

		return GenderAlignment.None;

	}

	private static async Task<IEnumerable<MetaVisual>> RecurseSubgroups(MetaSpecies species, JsonElement visualElement, IEnumerable<string> currentNames)
	{

		List<MetaVisual> visuals = [];

		// Work on this group.

		string? newName = visualElement.GetProperty("name").GetString()?.ToLowerInvariant();

		IEnumerable<string> newNameParts = string.IsNullOrWhiteSpace(newName) ? [] : newName.Split('-', '_');

		string[] names = [.. currentNames, .. newNameParts.Where(name => !string.IsNullOrWhiteSpace(name))]; // Also passed to recursion later.

		// Check if we have an excluded name. (e.g. cutscene)
		foreach (string name in names)
			if (EXCLUDED_GROUPS.Contains(name))
				return []; // Bail and return nothing. We don't want to process this group.

		visuals.AddRange(MaybeCreateMetaVisualsInPlace(species, visualElement, names));

		// Work on the subgroups of this group.

		JsonElement subgroups = visualElement.GetProperty("subgroups");

		foreach (JsonProperty property in subgroups.EnumerateObject())
		{

			JsonElement element = subgroups.GetProperty(property.Name);

			visuals.AddRange(await RecurseSubgroups(species, element, names));

		}

		return visuals;

	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="visualElement"></param>
	/// <param name="groupNames">Group names, INCLUDING THIS ONE!!</param>
	/// <returns></returns>
	private static List<MetaVisual> MaybeCreateMetaVisualsInPlace(MetaSpecies species, JsonElement visualElement, IEnumerable<string> groupNames)
	{

		List<MetaVisual> visuals = [];

		if (visualElement.GetProperty("sprite_required").GetBoolean())
			visuals.Add(new MetaSprites()
			{
				groupNames = groupNames,
				genderAlignment = BuildSpeciesUtils.HasGenderName(groupNames),
				isShiny = INameMatchable.NormalizeStringsForMatching(groupNames).Contains("shiny"),
				groupElement = visualElement,
				Species = species
			});

		if (visualElement.GetProperty("portrait_required").GetBoolean())
			visuals.Add(new MetaPortraits()
			{
				groupNames = groupNames,
				genderAlignment = BuildSpeciesUtils.HasGenderName(groupNames),
				isShiny = INameMatchable.NormalizeStringsForMatching(groupNames).Contains("shiny"),
				groupElement = visualElement,
				Species = species
			});

		return visuals;

	}

}
