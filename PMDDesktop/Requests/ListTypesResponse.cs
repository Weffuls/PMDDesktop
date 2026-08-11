namespace PMDDesktop.Requests;

public class ListTypesResponse : ServerResponse
{

	public class TypeEntry
	{

		public required string ID { get; init; }
		public required string[] Weaknesses { get; init; }
		public required string[] Resistances { get; init; }
		public required string[] Immunities { get; init; }

	}

	public required TypeEntry[] Types { get; init; }

}
