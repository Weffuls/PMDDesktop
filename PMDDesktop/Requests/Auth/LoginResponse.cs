namespace PMDDesktop.Requests.Auth;

public sealed class LoginResponse : ServerJsonResponse
{

	public required string AuthorizationToken { get; set; }

}
