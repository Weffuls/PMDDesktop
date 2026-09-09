using PMDDesktop.Server.Assets.Data;
using System.Collections.Immutable;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

internal abstract class MetaVisual : MetaAsset, INameMatchable
{

	protected AssetLocation GetVisualAssetLocation(string typeFolder)
	{

		IEnumerable<string> names = groupNames
			.Select(name => string.IsNullOrWhiteSpace(name) ? "default" : name)
			.Select(name => name.ToLowerInvariant());

		string groupFolderName = string.Join('-', names);

		return new("visuals", Species.DataName, groupFolderName, typeFolder);

	}

	public static readonly string[] EXCLUDED_FROM_MATCHING = ["shiny", "altcolor", "alternate", "male", "female", ""];

	public required MetaSpecies Species { get; init; }
	public required JsonElement groupElement;
	public required IEnumerable<string> groupNames;
	public List<MetaForm> ForForms { get; } = [];

	/// <summary>
	/// Does this Pokémon visual seem to align to a gender? For example, if a "{name}-female" exists, then the other one is probably a male, and we need to create two variants.
	/// </summary>
	public GenderAlignment genderAlignment = GenderAlignment.None;
	public required bool isShiny;

	public IEnumerable<string> GetMatchableParts() => groupNames.Where(name =>
	{

		string normalized = INameMatchable.NormalizeStringForMatching(name);

		return !EXCLUDED_FROM_MATCHING.Contains(normalized);

	});

	protected async Task CreateVisualAssetCommon(SpeciesVisual visual)
	{

		visual.Location = Location;
		visual.Shiny = isShiny;
		visual.forForms = ImmutableArray.Create([.. ForForms.Select(form => new AssetReference<SpeciesForm>(form.Location))]);

	}

}
