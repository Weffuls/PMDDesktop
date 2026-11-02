using PMDDesktop.Server.RequestHandlers;

namespace PMDDesktop.Server.RequestHandlers.Auth;

public sealed class ResponseResultLoginError() : ResponseResultError
{

	public override string GetErrorReason() => $"Invalid handle or password.";
	public override int GetStatusCode() => 400;

}
