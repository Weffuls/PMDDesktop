using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// This file contains extremely specialized helper functions for <see cref="BuildSpecies"/> relating to the <see href="https://github.com/PMDCollab/SpriteCollab">PMDCollab's SpriteCollab</see>.
/// They are delegated here to help <see cref="BuildSpecies"/> be easy to follow the flow of.
/// </summary>
internal static class SpriteCollabUtils
{

	/// <summary>
	/// Opinionated list of groups to skip.
	/// These groups don't make much sense in our program.
	/// </summary>
	public static readonly string[] EXCLUDED_GROUPS = ["cutscene", "skytemple"];

	/// <summary>
	/// Get the top element of a species in the tracker.json.
	/// </summary>
	/// <param name="species">The species to use to find the index.</param>
	/// <param name="zip">The <see cref="SpriteCollabZip"/> to use to get the tracker.json file.</param>
	/// <returns>A JsonElement of the species top in tracker.json.</returns>
	public static async Task<JsonElement> GetSpeciesTop(MetaSpecies species, SpriteCollabZip zip)
	{

		JsonElement tracker = await zip.GetTrackerJSON();

		return tracker.GetProperty(species.FormattedIndex);

	}

	/// <summary>
	/// Get all <see cref="MetaPortraits"/>s and <see cref="MetaSprites"/>s that would belong to <paramref name="species"/>, automatically assigning <see cref="GenderAlignment"/>s where needed.
	/// </summary>
	/// <param name="species">The species to examine to find <see cref="MetaVisual"/>s.</param>
	/// <returns>An enumerable of all found <see cref="MetaVisual"/>s.</returns>
	public static async Task<IEnumerable<MetaVisual>> GetAllVisuals(MetaSpecies species)
	{

		JsonElement top = await GetSpeciesTop(species, species.SpriteZip);

		List<MetaVisual> visuals = [.. await RecurseSubgroups(species, top, [])];

		// Make sure that each male/female has a counterpart.
		// This is run seperately for portraits & sprites so that they do not affect each other.
		// Some species have ungendered sprites but have gendered portraits.
		ReflectVisualGendersInPlace(visuals.OfType<MetaSprites>());
		ReflectVisualGendersInPlace(visuals.OfType<MetaPortraits>());

		return visuals;

	}

	/// <summary>
	/// <para>Attempts to match visuals with their counterparts, if one has a <see cref="GenderAlignment"/>, and another has <see cref="GenderAlignment.None"/>, then it is reflected to be the opposite of each other.</para>
	/// <para>This ensures that each male/female gender difference is matched with a counterpart instead of a male/none or none/female combo.</para>
	/// <para>This mutates the Enumerable in-place.</para>
	/// </summary>
	/// <param name="visuals">Enumerable of <see cref="MetaVisual"/>s to perform this reflection on. The <see cref="MetaVisual"/>s will be modified in-place.</param>
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
	/// Returns the "reflected" or "opposite" <see cref="GenderAlignment"/>. That is, <see cref="GenderAlignment.Male"/> into <see cref="GenderAlignment.Female"/> and <see cref="GenderAlignment.Female"/> into <see cref="GenderAlignment.Male"/>.
	/// </summary>
	/// <param name="gender">The <see cref="GenderAlignment"/> to reflect.</param>
	/// <returns>The "reflected" or "opposite" <see cref="GenderAlignment"/> of the input.</returns>
	private static GenderAlignment GetReflectedGender(GenderAlignment gender)
	{

		if (gender == GenderAlignment.Male)
			return GenderAlignment.Female;

		if (gender == GenderAlignment.Female)
			return GenderAlignment.Male;

		return GenderAlignment.None;

	}

	/// <summary>
	/// Add this <paramref name="visualElement"/> as a <see cref="MetaPortraits"/> and/or <see cref="MetaSprites"/> if applicable, then recurse into the "subgroups" found to add those too.
	/// </summary>
	/// <param name="species"></param>
	/// <param name="visualElement"></param>
	/// <param name="currentNames"></param>
	/// <returns></returns>
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
	/// If needed, create a <see cref="MetaSprites"/> and a <see cref="MetaPortraits"/> based on <paramref name="visualElement"/>.
	/// </summary>
	/// <param name="species"><see cref="MetaSpecies"/> that the created <see cref="MetaVisual"/>s will have their <see cref="MetaVisual.Species"/> property set to.</param>
	/// <param name="visualElement">The element to create the <see cref="MetaPortraits"/> and <see cref="MetaSprites"/> from.</param>
	/// <param name="groupNames">Group names, INCLUDING THIS ONE!!</param>
	/// <returns>Returns an enumerable containing any <see cref="MetaVisual"/>s that were created (if any).</returns>
	/// <remarks>You must include all group names, including past ones and the current one that this <paramref name="visualElement"/> contains.</remarks>
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
