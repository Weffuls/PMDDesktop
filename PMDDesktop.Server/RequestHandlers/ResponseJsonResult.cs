using PMDDesktop.Requests;

namespace PMDDesktop.Server.RequestHandlers;

public sealed class ResponseJsonResult<TJsonResponse, TResponseError> : ResponseResult<TJsonResponse, TResponseError> where TJsonResponse : ServerJsonResponse where TResponseError : ResponseResultError
{

	public ResponseJsonResult(TResponseError error) : base(error) { }
	public ResponseJsonResult(TJsonResponse result) : base(result) { }

}
