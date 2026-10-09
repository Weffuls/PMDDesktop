using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class CreateRoleRequestHandler : JsonRequestHandler<CreateRoleRequest, RoleInfoResponse>
{

	protected override async Task<RoleInfoResponse> CreateResponse(CreateRoleRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		// To create roles, you need the MANAGE_ROLES permission.
		if (!requestingUser.HasPermission(UserPermission.MANAGE_ROLES))
			throw new UserRequestException($"You need permission to manage roles to access this endpoint.");

		UserRole role = await game.State.Roles.TryCreateRole()
			?? throw new Exception($"Unable to create role.");

		if (request.NewName is not null)
			await role.SetName(request.NewName);

		return GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role);

	}

}
