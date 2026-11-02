namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that an exception occurred while processing the request.
/// </summary>
/// <param name="exception">The exception that caused this to be returned.</param>
/// <remarks>
/// If the exception that occurred can be avoided, but still invalidates the request, try to avoid it and return a different <see cref="ResponseResultError"/> instead.
/// </remarks>
public sealed class ResponseResultExceptionError(Exception exception) : ResponseResultError
{

	/// <summary>
	/// The exception that caused this to be returned.
	/// </summary>
	public Exception Exception { get; private init; } = exception;
	public override string GetErrorReason() => $"Server-side exception of type '{Exception.GetType().Name}' with message: \"{Exception.Message}\" This is likely a bug with the server and should be reported.";
	public override int GetStatusCode() => 500;

}
