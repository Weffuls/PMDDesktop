namespace PMDDesktop.Requests.PokemonTypes;

public class ListPokemonTypesResponse : ServerJsonResponse
{

	public class TypeEntry
	{

		public required DataIDPath ID { get; init; }
		public required string[] Weaknesses { get; init; }
		public required string[] Resistances { get; init; }
		public required string[] Immunities { get; init; }

	}

	public required TypeEntry[] Types { get; init; }

}
