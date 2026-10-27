using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class ListUsersRequestHandler : JsonRequestHandler<ListUsersRequest, ListUsersResponse>
{

	protected override async Task<ResponseJsonResult<ListUsersResponse, ResponseResultError>> CreateResponse(ListUsersRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error1))
			return new(error1);

		return new(new ListUsersResponse()
		{
			Users = [.. game.State.Users.Select(user => GenericResponseCreators.CreateUserInfoResponse(requestingUser, user))]
		});

	}

}
