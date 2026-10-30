using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that the user is not high enough in the Role Hierarchy to perform an operation on another <see cref="IUserHierarchyComparable"/>.
/// </summary>
public class ResponseResultHierarchyError() : ResponseResultError
{

	public override string GetErrorReason() => $"You are not high enough in the role hierarchy to perform this operation.";
	public override int GetStatusCode() => 403;

}
