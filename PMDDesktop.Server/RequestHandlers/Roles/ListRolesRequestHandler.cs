using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class ListRolesRequestHandler : JsonRequestHandler<ListRolesRequest, ListRolesResponse>
{

	protected override async Task<ResponseJsonResult<ListRolesResponse, ResponseResultError>> CreateResponse(ListRolesRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error))
			return new(error);

		return new(new ListRolesResponse()
		{
			Roles = [.. game.State.Roles.Select(role => GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role))]
		});

	}

}
