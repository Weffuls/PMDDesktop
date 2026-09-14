using PMDDesktop.GameData;

namespace PMDDesktop.Tests.Utils;

public class ExamplePokemonType : IPokemonType
{

	public List<IPokemonType> Weaknesses { get; set; } = [];
	public List<IPokemonType> Resistances { get; set; } = [];
	public List<IPokemonType> Immunities { get; set; } = [];

	IEnumerable<IPokemonType> IPokemonType.Weaknesses => Weaknesses;
	IEnumerable<IPokemonType> IPokemonType.Resistances => Resistances;
	IEnumerable<IPokemonType> IPokemonType.Immunities => Immunities;

}
