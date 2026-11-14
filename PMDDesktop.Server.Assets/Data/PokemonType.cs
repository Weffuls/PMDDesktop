using System.Collections.Immutable;
using System.Text.Json.Serialization;
using PMDDesktop.GameData;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("type")]
public sealed class PokemonType : Asset, IPokemonType
{

	internal static DataID TYPES_ID = new("types"u8);

	internal static DataIDPath DefaultTypeLocation(string typeName)
	{
		return new(TYPES_ID, (DataID)typeName);
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
	internal PokemonType(DataIDPath path) : base(path) { }

}
