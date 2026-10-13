namespace PMDDesktop.Requests.Auth;

public sealed class WhoAmIResponse : ServerResponse
{

	public required Guid GUID {get; set;}

}
