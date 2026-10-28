namespace PMDDesktop.Server.RequestHandlers;

public sealed class ResponseResultExceptionError(Exception exception) : ResponseResultError
{

	public Exception Exception { get; private init; } = exception;
	public override string GetErrorReason() => $"Server-side exception of type '{Exception.GetType().Name}' with message: \"{Exception.Message}\" This is likely a bug with the server and should be reported.";
	public override int GetReturnCode() => 500;

}
