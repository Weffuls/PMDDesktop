using System.Collections.Immutable;
using System.Text.Json.Serialization;
using PMDDesktop.GameData;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("form")]
public sealed class SpeciesForm : Asset
{

	public ImmutableArray<AssetReference<PokemonType>> Types { get; internal init; } = [];
	public BattleStats Stats { get; internal set; }

	[JsonConstructor]
	internal SpeciesForm(DataIDPath path) : base(path) { }

}
