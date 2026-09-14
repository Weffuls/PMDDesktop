using PMDDesktop.GameData;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("form")]
public sealed class SpeciesForm : Asset
{

	public ImmutableArray<AssetReference<PokemonType>> Types { get; internal init; } = [];
	public BattleStats Stats { get; internal set; }

	internal SpeciesForm(AssetLocation location) : base(location) { }

	[JsonConstructor]
	private SpeciesForm() : base() { }

}
