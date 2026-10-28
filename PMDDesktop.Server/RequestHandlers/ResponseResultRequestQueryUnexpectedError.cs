namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultRequestQueryUnexpectedError(string queryKey) : ResponseResultRequestError
{

	public string Key { get; private init; } = queryKey;

	protected override string GetRequestErrorReason() => $"Query parameter '{Key}' was provided, but is unexpected. Check the spelling of the parameter, or do not include it in your request.";

}
