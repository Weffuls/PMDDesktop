using PMDDesktop.Results;

namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResult<TResult, TError> : Result<TResult, TError> where TError : ResponseResultError
{

	public ResponseResult(TError error) : base(error) { }
	public ResponseResult(TResult result) : base(result) { }

}
