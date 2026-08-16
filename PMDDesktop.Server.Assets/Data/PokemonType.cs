using PMDDesktop.GameData;
using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("type")]
public class PokemonType : Asset, IPokemonType
{

	internal static AssetLocation DefaultTypeLocation(string typeName)
	{
		return new("types", typeName);
	}

	public ImmutableArray<AssetReference<PokemonType>> Weaknesses { get; internal set; } = [];
	public ImmutableArray<AssetReference<PokemonType>> Resistances { get; internal set; } = [];
	public ImmutableArray<AssetReference<PokemonType>> Immunities { get; internal set; } = [];

	IEnumerable<IPokemonType> IPokemonType.Weaknesses =>
		Weaknesses.Select((reference) =>
			reference.GetReference(Manager ?? throw new NullReferenceException($"Can't resolve references of Weaknesses of {this} because Manager is null.")
		));

	IEnumerable<IPokemonType> IPokemonType.Resistances =>
		Resistances.Select((reference) =>
			reference.GetReference(Manager ?? throw new NullReferenceException($"Can't resolve references of Resistances of {this} because Manager is null.")
		));

	IEnumerable<IPokemonType> IPokemonType.Immunities =>
		Resistances.Select((reference) =>
			reference.GetReference(Manager ?? throw new NullReferenceException($"Can't resolve references of Immunities of {this} because Manager is null.")
		));

	[JsonConstructor]
	private PokemonType() : this(new()) { }

	internal PokemonType(AssetLocation location) : base(location) { }

}
