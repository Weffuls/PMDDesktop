namespace PMDDesktop.Requests.Species;

public class ListSpeciesResponse : ServerJsonResponse
{

	public required string[] SpeciesIDs { get; init; }

}
