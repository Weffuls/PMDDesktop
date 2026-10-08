using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class ListRolesRequestHandler : JsonRequestHandler<ListRolesRequest, ListRolesResponse>
{

	protected override async Task<ListRolesResponse> CreateResponse(ListRolesRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		return new()
		{
			Roles = [.. game.State.Roles.Select(role => GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role))]
		};

	}

}
