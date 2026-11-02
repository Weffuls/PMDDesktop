using PMDDesktop.Results;

namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResult<TSuccess, TError> : Result<TSuccess, TError> where TError : ResponseResultError
{

	/// <summary>
	/// <para>Respond that a Request is unsuccessful or errored.</para>
	/// </summary>
	/// <remarks>
	/// <para>Semantically, this means the Request was recieved and our response is that it's unsuccessful.</para>
	/// <para>We cannot take action due to some circumstance, like bad state, bad request, or no permissions.</para>
	/// </remarks>
	/// <param name="error">The <see cref="TError"/> to hold.</param>
	public ResponseResult(TError error) : base(error) { }

	/// <summary>
	/// <para>Respond that a Request is successful and action was taken or info was returned.</para>
	/// </summary>
	/// <remarks>
	/// <para>Semantically, this means the Request was recieved and we were able to complete the request.</para>
	/// <para>If for some reason, the request could not be completed, an error should be returned instead.</para>
	/// </remarks>
	/// <param name="success">The <see cref="TSuccess"/> to hold.</param>
	public ResponseResult(TSuccess success) : base(success) { }

}
