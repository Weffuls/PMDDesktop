namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that a key in the JSON body should've been provided as a URL Query Parameter.
/// </summary>
/// <param name="queryKey">The name of the key that should be passed as a URL Query Parameter.</param>
public class ResponseResultRequestQueryMisplacedError(string queryKey) : ResponseResultRequestError
{

	/// <summary>
	/// The name of the key that should be passed as a URL Query Parameter.
	/// </summary>
	public string Key { get; private init; } = queryKey;

	protected override string GetRequestErrorReason() => $"Query parameter '{Key}' was provided in the JSON, but is expected to be provided as part of the URL query.";

}
