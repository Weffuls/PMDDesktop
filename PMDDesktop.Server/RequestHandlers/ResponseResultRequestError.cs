namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// Abstract class for all errors that occur because the inputs cannot be understood.
/// </summary>
public abstract class ResponseResultRequestError() : ResponseResultError
{

	public override string GetErrorReason() => $"The data you sent is invalid or could not be parsed: {GetRequestErrorReason()}";
	protected abstract string GetRequestErrorReason();
	public override int GetReturnCode() => 400;

}
