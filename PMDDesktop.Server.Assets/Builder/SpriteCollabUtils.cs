using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// This file contains extremely specialized helper functions for BuildSpecies relating to the PMDCollab's SpriteCollab.
/// They are delegated here to help BuildSpecies be easy to follow the flow of.
/// </summary>
internal static class SpriteCollabUtils
{

	public static async Task<JsonElement> GetSpeciesTop(string index, SpriteCollabZip zip)
	{

		JsonElement tracker = await zip.GetTrackerJSON();

		return tracker.GetProperty(index);

	}

	public static async Task<IEnumerable<MetaVisual>> GetAllVisuals(MetaSpecies species)
	{

		JsonElement top = await GetSpeciesTop(species.FormattedIndex, species.SpriteZip);

		return await RecurseSubgroups(species, top, []);

	}

	private static async Task<IEnumerable<MetaVisual>> RecurseSubgroups(MetaSpecies species, JsonElement visualElement, IEnumerable<string> currentNames)
	{

		List<MetaVisual> visuals = [];

		JsonElement subgroups = visualElement.GetProperty("subgroups");
		foreach (JsonProperty property in subgroups.EnumerateObject())
		{

			JsonElement element = subgroups.GetProperty(property.Name);

			string newName = (element.GetProperty("name").GetString() ?? throw new InvalidDataException($"{element}'s name is not a string?")).ToLowerInvariant();

			IEnumerable<string> newNameParts = newName.Split('-', '_');

			string[] names = [.. currentNames, .. newNameParts];

			visuals.AddRange(CreateMetaVisualsInPlace(species, element, names));
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
	private static List<MetaVisual> CreateMetaVisualsInPlace(MetaSpecies species, JsonElement visualElement, IEnumerable<string> groupNames)
	{

		List<MetaVisual> visuals = [];

		visuals.Add(new MetaSprites()
		{
			groupNames = groupNames,
			genderAlignment = BuildSpeciesUtils.HasGenderName(groupNames),
			isShiny = INameMatchable.NormalizeStringsForMatching(groupNames).Contains("shiny"),
			groupElement = visualElement,
			Species = species
		});

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
