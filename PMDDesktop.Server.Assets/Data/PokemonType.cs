using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("type")]
public class PokemonType : Asset
{

	internal static AssetLocation DefaultTypeLocation(string typeName)
	{
		return new("types", typeName);
	}

	public ImmutableArray<AssetReference<PokemonType>> Weaknesses { get; internal set; } = [];
	public ImmutableArray<AssetReference<PokemonType>> Resistances { get; internal set; } = [];
	public ImmutableArray<AssetReference<PokemonType>> Immunities { get; internal set; } = [];

	[JsonConstructor]
	private PokemonType() : this(new()) { }

	internal PokemonType(AssetLocation location) : base(location) { }

}
