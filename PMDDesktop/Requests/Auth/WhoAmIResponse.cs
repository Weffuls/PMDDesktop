namespace PMDDesktop.Requests.Auth;

public sealed class WhoAmIResponse : ServerJsonResponse
{

	public required Guid GUID {get; set;}

}
