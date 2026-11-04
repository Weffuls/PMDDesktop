namespace PMDDesktop.Requests.Auth;

[ServerRequest("/api/auth/login")]
public sealed class LoginRequest : ServerRequest<LoginResponse>
{

	public required DataID Handle { get; set; }
	public required string Password { get; set; }

}
