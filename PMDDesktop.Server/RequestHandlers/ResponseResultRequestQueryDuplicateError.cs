namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultRequestQueryDuplicateError(string queryKey, int count) : ResponseResultRequestError
{

	public string Key { get; private init; } = queryKey;
	public int Count { get; private init; } = count;

	protected override string GetRequestErrorReason() => $"Query parameter '{Key}' was provided multiple times ({Count} times); each query parameter should be unique.";

}
