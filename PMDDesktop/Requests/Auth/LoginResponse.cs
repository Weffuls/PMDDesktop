namespace PMDDesktop.Requests.Auth;

public sealed class LoginResponse : ServerResponse
{

	public required string AuthorizationToken { get; set; }

}
