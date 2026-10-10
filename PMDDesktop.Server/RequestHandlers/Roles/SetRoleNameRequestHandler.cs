using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class SetRoleNameRequestHandler : JsonRequestHandler<SetRoleNameRequest, RoleInfoResponse>
{

	protected override async Task<RoleInfoResponse> CreateResponse(SetRoleNameRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		if (!requestingUser.HasPermission(UserPermission.MANAGE_ROLES))
			throw new UserRequestException($"You need permission to manage roles to access this endpoint.");

		if (!game.TryGetRole(request.GUID, out UserRole? role))
			throw new UserRequestException($"{request.GUID} does not match a role.");

		if (!requestingUser.HigherThan(role))
			throw new UserRequestException($"You are not high enough in the role hierarchy to perform this operation.");

		await role.SetName(request.NewName);

		return GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role);

	}

}
