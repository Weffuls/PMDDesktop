namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultRequestQueryMisplacedError(string queryKey) : ResponseResultRequestError
{

	public string Key { get; private init; } = queryKey;

	protected override string GetRequestErrorReason() => $"Query parameter '{Key}' was provided in the JSON, but is expected to be provided as part of the URL query.";

}
