namespace PMDDesktop.Requests;

public class ListTypesResponse : ServerResponse
{

	public required string[] TypeIDs { get; init; }

}
