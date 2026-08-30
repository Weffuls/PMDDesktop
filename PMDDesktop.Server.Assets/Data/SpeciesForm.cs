using PMDDesktop.GameData;
using System.Collections.Immutable;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("form")]
public class SpeciesForm : Asset
{

	public ImmutableArray<AssetReference<PokemonType>> Types { get; internal init; } = [];
	public BattleStats Stats { get; internal set; }

	internal SpeciesForm(AssetLocation location) : base(location)
	{



	}

}
