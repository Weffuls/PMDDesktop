using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class CreateRoleRequestHandler : JsonRequestHandler<CreateRoleRequest, RoleInfoResponse>
{

	protected override async Task<ResponseResult<RoleInfoResponse, ResponseResultError>> CreateResponse(CreateRoleRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [UserPermission.MANAGE_ROLES], out ResponseResultError? error))
			return new(error);

		UserRole role = await game.State.Roles.TryCreateRole()
			?? throw new Exception($"Unable to create role.");

		if (request.NewName is not null)
			await role.SetName(request.NewName);

		return new(GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role));

	}

}
