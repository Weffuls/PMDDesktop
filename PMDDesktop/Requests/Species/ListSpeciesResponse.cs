namespace PMDDesktop.Requests.Species;

public class ListSpeciesResponse : ServerJsonResponse
{

	public required DataIDPath[] SpeciesIDs { get; init; }

}
