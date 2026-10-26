namespace PMDDesktop.Server.RequestHandlers;

public sealed class ResponseResultExceptionError(Exception exception) : ResponseResultError
{

	public Exception Exception { get; private init; } = exception;
	public override string GetErrorReason() => $"Server-side exception of type '{Exception.GetType().Name}' with message: {Exception.Message}";
	public override int GetReturnCode() => 500;

}
