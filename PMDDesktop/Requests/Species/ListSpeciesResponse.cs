namespace PMDDesktop.Requests.Species;

public class ListSpeciesResponse : ServerResponse
{

	public required string[] SpeciesIDs { get; init; }

}
