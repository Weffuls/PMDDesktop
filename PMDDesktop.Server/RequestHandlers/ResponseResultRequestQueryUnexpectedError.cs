namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that the query parameters provided in the URL contain a key not included in the schema.
/// </summary>
/// <param name="queryKey">The name of the key that was unexpectedly provided.</param>
public class ResponseResultRequestQueryUnexpectedError(string queryKey) : ResponseResultRequestError
{

	/// <summary>
	/// The name of the key that was unexpectedly provided.
	/// </summary>
	public string Key { get; private init; } = queryKey;

	protected override string GetRequestErrorReason() => $"Query parameter '{Key}' was provided, but is unexpected. Check the spelling of the parameter, or do not include it in your request.";

}
