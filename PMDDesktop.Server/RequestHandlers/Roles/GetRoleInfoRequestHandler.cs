using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class GetRoleInfoRequestHandler : JsonRequestHandler<GetRoleInfoRequest, RoleInfoResponse>
{

	protected override async Task<RoleInfoResponse> CreateResponse(GetRoleInfoRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		if (!game.TryGetRole(request.GUID, out UserRole? role))
			throw new UserRequestException($"{request.GUID} does not match a role.");

		return GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role);

	}

}
