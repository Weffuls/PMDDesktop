namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultAuthenticationError(string? providedTokenString) : ResponseResultError
{

	public string? ProvidedTokenString { get; private init; } = providedTokenString;
	public override string GetErrorReason()
	{

		return ProvidedTokenString is not null
			? $"The provided token string does not authenticate you. Your login may have expired, or it may be formatted incorrectly."
			: $"You need to be authenicated to access this endpoint. Please provide a valid token in the 'Authorization' header.";

	}

	public override int GetReturnCode() => 401;

}
