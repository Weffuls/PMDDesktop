namespace PMDDesktop.Requests;

public class ListSpeciesResponse : ServerResponse
{

	public required string[] SpeciesIDs { get; init; }

}
