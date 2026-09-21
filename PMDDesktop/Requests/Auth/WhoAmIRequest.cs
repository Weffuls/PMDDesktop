namespace PMDDesktop.Requests.Auth;

[ServerRequest("/api/auth/who-am-i")]
public sealed class WhoAmIRequest : ServerRequest<WhoAmIResponse>
{

	// Expecting Authorization header for this request.

}
