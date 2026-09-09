using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

internal abstract class MetaVisual : MetaAsset, INameMatchable
{

	protected AssetLocation GetVisualAssetLocation(string typeFolder)
	{

		string groupFolderName = string.Join('-', groupNames);

		return new("visuals", Species.DataName, groupFolderName, typeFolder);

	}

	public static readonly string[] EXCLUDED_FROM_MATCHING = ["shiny", "altcolor", "alternate"];

	public required MetaSpecies Species { get; init; }
	public required JsonElement groupElement;
	public required IEnumerable<string> groupNames;

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

	internal override async Task<Asset> CreateAsset()
	{
		throw new NotImplementedException();
	}

}
