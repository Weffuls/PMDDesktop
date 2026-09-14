namespace PMDDesktop.GameData;

public interface IPokemonType
{

	IEnumerable<IPokemonType> Weaknesses { get; }
	IEnumerable<IPokemonType> Resistances { get; }
	IEnumerable<IPokemonType> Immunities { get; }

}
