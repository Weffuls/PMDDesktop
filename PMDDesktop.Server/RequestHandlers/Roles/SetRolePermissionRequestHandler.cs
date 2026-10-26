using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class SetRolePermissionRequestHandler : JsonRequestHandler<SetRolePermissionRequest, RoleInfoResponse>
{

	protected override async Task<ResponseJsonResult<RoleInfoResponse, ResponseResultError>> CreateResponse(SetRolePermissionRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [UserPermission.MANAGE_ROLES], out ResponseResultError? error1))
			return new(error1);

		if (!CommonResponseHandlerChecks.TryGetRole(game, request.GUID, out UserRole? role, out ResponseResultError? error2))
			return new(error2);

		if (!CommonResponseHandlerChecks.TryCompareHierarchy(requestingUser, role, out ResponseResultError? error3))
			return new(error3);

		if (!CommonResponseHandlerChecks.TryGetPermission(request.PermissionDataName, out UserPermission? permission, out ResponseResultError? error4))
			return new(error4);

		await role.SetPermission(permission, request.NewPermissionValue);

		return new(GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role));

	}

}
