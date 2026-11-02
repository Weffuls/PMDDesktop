using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that the user is not high enough in the Role Hierarchy to perform an operation on another <see cref="IUserHierarchyComparable"/>.
/// </summary>
public class ResponseResultHierarchyError(User user, IUserHierarchyComparable comparedTo) : ResponseResultError
{

	public override string GetErrorReason() => $"You ({user.GetClientFacingName()} ({user.GetHierarchyPosition()})) are not high enough in the role hierarchy to perform an operation on {comparedTo.GetClientFacingName()} ({comparedTo.GetHierarchyPosition()}).";
	public override int GetStatusCode() => 403;

}
