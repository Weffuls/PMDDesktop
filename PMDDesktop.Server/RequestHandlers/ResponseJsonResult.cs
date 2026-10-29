using PMDDesktop.Requests;

namespace PMDDesktop.Server.RequestHandlers;

public sealed class ResponseJsonResult<TJsonResponse, TResponseError> : ResponseResult<TJsonResponse, TResponseError> where TJsonResponse : ServerJsonResponse where TResponseError : ResponseResultError
{

	/// <summary>
	/// <para>Respond that a Request is unsuccessful or errored.</para>
	/// </summary>
	/// <remarks>
	/// <para>Semantically, this means the Request was recieved and our response is that it's unsuccessful.</para>
	/// <para>If inside <see cref="JsonRequestHandler.CreateResponse(TReq, Game.GameServer, Server.Users.User?)"/>, the provided <paramref name="error"/> will be sent back to the client.</para>
	/// </remarks>
	/// <param name="error">The <see cref="TError"/> to send back to the client.</param>
	public ResponseJsonResult(TResponseError error) : base(error) { }

	/// <summary>
	/// <para>Respond that a Request is successful and this is the object produced by the result.</para>
	/// </summary>
	/// <remarks>
	/// <para>Semantically, this means the Request was recieved and we were able to complete the request.</para>
	/// <para>If inside <see cref="JsonRequestHandler.CreateResponse(TReq, Game.GameServer, Server.Users.User?)"/>, the provided <paramref name="success"/> will be sent back to the client.</para>
	/// </remarks>
	/// <param name="success">The <see cref="TJsonResponse"/> to send back to the client.</param>
	public ResponseJsonResult(TJsonResponse success) : base(success) { }

}
