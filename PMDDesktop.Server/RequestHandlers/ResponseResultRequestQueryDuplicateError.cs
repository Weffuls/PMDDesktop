namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that the query parameters provided in the URL contain multiple of the same key.
/// </summary>
/// <param name="queryKey">The name of the key that was duplicated.</param>
/// <param name="count">The amount of times total that the key was provided.</param>
public class ResponseResultRequestQueryDuplicateError(string queryKey, int count) : ResponseResultRequestError
{

	/// <summary>
	/// The name of the key that was duplicated.
	/// </summary>
	public string Key { get; private init; } = queryKey;

	/// <summary>
	/// The amount of times total that the key was provided.
	/// </summary>
	public int Count { get; private init; } = count;

	protected override string GetRequestErrorReason() => $"Query parameter '{Key}' was provided multiple times ({Count} times); each query parameter should be unique.";

}
