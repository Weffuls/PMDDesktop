using System.Collections.Immutable;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("variant")]
public class SpeciesVariant : Asset
{

	public required AssetReference<Species> Species { get; init; }
	public required AssetReference<SpeciesForm> DefaultForm { get; init; }
	public required ImmutableArray<string> EvolutionTags { get; init; }
	public required ImmutableArray<AssetReference<SpeciesForm>> OtherForms { get; init; }
	public ImmutableArray<string> SpecialFlags { get; init; } = [];

	internal SpeciesVariant(AssetLocation location) : base(location)
	{



	}

}
