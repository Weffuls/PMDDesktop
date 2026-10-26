using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultHierarchyError() : ResponseResultError
{

	public override string GetErrorReason() => $"You are not high enough in the role hierarchy to perform this operation.";
	public override int GetReturnCode() => 403;

}
