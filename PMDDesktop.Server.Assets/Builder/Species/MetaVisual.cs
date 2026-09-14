using PMDDesktop.Server.Assets.Data;
using System.Collections.Immutable;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="SpeciesVisual"/> before creating it.
/// </summary>
internal abstract class MetaVisual : MetaAsset, INameMatchable
{

	/// <summary>
	/// Function to help determine <see cref="MetaAsset.Location"/> for <see cref="MetaVisual"/>s.
	/// </summary>
	/// <param name="typeFolder">The name of the folder to distinguish this asset's type, for example "portraits" or "sprites".</param>
	/// <returns></returns>
	protected AssetLocation GetVisualAssetLocation(string typeFolder)
	{

		IEnumerable<string> names = groupNames
			.Select(name => string.IsNullOrWhiteSpace(name) ? "default" : name)
			.Select(name => name.ToLowerInvariant());

		string groupFolderName = string.Join('-', names);

		return new(Species.Location, typeFolder, groupFolderName);

	}

	/// <summary>
	/// <para>List of strings that won't be included when participating as a <see cref="INameMatchable"/>.</para>
	/// <para>These strings have a special meaning that doesn't make sense when matching names.</para>
	/// </summary>
	public static readonly string[] EXCLUDED_FROM_MATCHING = ["shiny", "altcolor", "alternate", "male", "female"];

	/// <summary>
	/// The species that this <see cref="MetaVisual"/> belongs to.
	/// </summary>
	public required MetaSpecies Species { get; init; }

	/// <summary>
	/// The group element that this <see cref="MetaVisual"/> originated from.
	/// </summary>
	public required JsonElement groupElement;

	/// <summary>
	/// An enumerable of group names that can be used to name and match this <see cref="MetaVisual"/>.
	/// </summary>
	public required IEnumerable<string> groupNames;

	/// <summary>
	/// <para>A list of <see cref="MetaForm"/>s that this <see cref="MetaVisual"/> belongs to.</para>
	/// </summary>
	public List<MetaForm> ForForms { get; } = [];

	/// <summary>
	/// Does this Pokémon visual seem to align to a gender? For example, if a "{name}-female" exists, then the other one is probably a male, and we need to create two variants.
	/// </summary>
	public GenderAlignment genderAlignment = GenderAlignment.None;

	/// <summary>
	/// Is this Pokémon visual shiny?
	/// </summary>
	public required bool isShiny;

	public IEnumerable<string> GetMatchableParts() => groupNames.Where(name =>
	{

		string normalized = INameMatchable.NormalizeStringForMatching(name);

		return !EXCLUDED_FROM_MATCHING.Contains(normalized);

	});

	/// <summary>
	/// Drop references found in <see cref="MetaVisual.ForForms"/> to <see cref="MetaForm"/>s that are no longer referenced by the linked <see cref="MetaSpecies"/>.
	/// </summary>
	public void DropUnreferencedForms()
	{

		ForForms.RemoveAll(form => !Species.metaAssets.Contains(form));

	}

	/// <summary>
	/// <para>Common functionality when turing a <see cref="MetaVisual"/> into a <see cref="SpeciesVisual"/>/<see cref="Asset"/>.</para>
	/// </summary>
	/// <param name="visual">The <see cref="SpeciesVisual"/> to modify.</param>
	/// <returns>Completes Task when modifications are complete.</returns>
	/// <remarks>
	/// Note: edits the <paramref name="visual"/> <b>in-place</b>!
	/// </remarks>
	protected async Task CreateVisualAssetCommon(SpeciesVisual visual)
	{

		visual.Location = Location;
		visual.Shiny = isShiny;
		visual.ForForms = ImmutableArray.Create([.. ForForms.Select(form => new AssetReference<SpeciesForm>(form.Location))]);

	}

}
