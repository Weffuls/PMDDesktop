namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// Abstract class for all <see cref="ResponseResultError"/>s that occur because the inputs cannot be understood.
/// </summary>
public abstract class ResponseResultRequestError() : ResponseResultError
{

	public override string GetErrorReason() => $"The data you sent is invalid or could not be parsed: {GetRequestErrorReason()}";

	/// <summary>
	/// <para>Create a string describing why this request error happened.</para>
	/// </summary>
	/// <returns>A string describing why this request error happened.</returns>
	/// <remarks>
	/// This will be appended after a generic error stating that the data input was invalid.
	/// </remarks>
	protected abstract string GetRequestErrorReason();
	public override int GetStatusCode() => 400;

}
