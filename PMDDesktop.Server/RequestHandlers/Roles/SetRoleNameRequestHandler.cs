using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class SetRoleNameRequestHandler : JsonRequestHandler<SetRoleNameRequest, RoleInfoResponse>
{

	protected override async Task<ResponseJsonResult<RoleInfoResponse, ResponseResultError>> CreateResponse(SetRoleNameRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [UserPermission.MANAGE_ROLES], out ResponseResultError? error1))
			return new(error1);

		if (!CommonResponseHandlerChecks.TryGetRole(game, request.GUID, out UserRole? role, out ResponseResultError? error2))
			return new(error2);

		if (!CommonResponseHandlerChecks.TryCompareHierarchy(requestingUser, role, out ResponseResultError? error3))
			return new(error3);

		await role.SetName(request.NewName);

		return new(GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role));

	}

}
