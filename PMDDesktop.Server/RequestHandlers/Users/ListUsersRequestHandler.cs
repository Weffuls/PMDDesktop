using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class ListUsersRequestHandler : JsonRequestHandler<ListUsersRequest, ListUsersResponse>
{

	protected override async Task<ListUsersResponse> CreateResponse(ListUsersRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		return new()
		{
			Users = [.. game.State.Users.Select(user => GenericResponseCreators.CreateUserInfoResponse(requestingUser, user))]
		};

	}

}
