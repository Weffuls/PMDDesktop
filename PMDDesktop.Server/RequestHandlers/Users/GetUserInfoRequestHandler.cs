using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class GetUserInfoRequestHandler : JsonRequestHandler<GetUserInfoRequest, GetUserInfoResponse>
{

	protected override async Task<GetUserInfoResponse> CreateResponse(GetUserInfoRequest request, GameServer game, User? requestingUser)
	{

		if (!game.TryGetUser(request.GUID, out User? user))
			throw new UserRequestException($"{request.GUID} does not match a user.");

		return new()
		{
			DisplayName = user.Name,
			GUID = user.GUID
		};

	}

}
