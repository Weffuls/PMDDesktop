using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class GetUserInfoRequestHandler : JsonRequestHandler<GetUserInfoRequest, UserInfoResponse>
{

	protected override async Task<UserInfoResponse> CreateResponse(GetUserInfoRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		if (!game.TryGetUser(request.GUID, out User? user))
			throw new UserRequestException($"{request.GUID} does not match a user.");

		return GenericResponseCreators.CreateUserInfoResponse(requestingUser, user);

	}

}
