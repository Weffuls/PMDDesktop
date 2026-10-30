namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that specifies that the user is not authenticated or could not be authenticated. (Cannot log in.)
/// </summary>
/// <param name="providedTokenString">Optionally take in the provided token string to state that the specific token string didn't work.</param>
public class ResponseResultAuthenticationError(string? providedTokenString) : ResponseResultError
{

	/// <summary>
	/// The token string that was provided that failed to log in.
	/// </summary>
	public string? ProvidedTokenString { get; private init; } = providedTokenString;
	public override string GetErrorReason()
	{

		return ProvidedTokenString is not null
			? $"The provided token string does not authenticate you. Your login may have expired, or it may be formatted incorrectly."
			: $"You need to be authenicated to access this endpoint. Please provide a valid token in the 'Authorization' header.";

	}

	public override int GetStatusCode() => 401;

}
