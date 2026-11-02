namespace PMDDesktop.Results;

/// <summary>
/// <para>Generic <see cref="ResultError"/> for any error that occurs because of an Exception.</para>
/// </summary>
/// <param name="exception"></param>
public class ResultExceptionError(Exception exception) : ResultError
{

	/// <summary>
	/// The exception that resulted in this <see cref="ResultExceptionError"/>.
	/// </summary>
	public Exception Exception { get; private init; } = exception;

	public override string GetErrorReason() => $"Exception occurred: '{Exception.Message}'";

}
