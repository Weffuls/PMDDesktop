namespace PMDDesktop.Results;

public class ResultExceptionError(Exception exception) : ResultError
{

	public Exception Exception { get; private init; } = exception;

	public override string GetErrorReason() => $"Exception occurred: '{Exception.Message}'";

}
