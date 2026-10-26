using PMDDesktop.Results;

namespace PMDDesktop.Server.RequestHandlers;

public abstract class ResponseResultError : ResultError
{

	public abstract int GetReturnCode();

}
