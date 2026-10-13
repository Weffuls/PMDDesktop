using PMDDesktop.Requests.Auth;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Auth;

public class WhoAmIRequestHandler : JsonRequestHandler<WhoAmIRequest, WhoAmIResponse>
{

	protected override async Task<WhoAmIResponse> CreateResponse(WhoAmIRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"Please send your valid token in the Authorization header to access this endpoint.");

		return new()
		{
			GUID = requestingUser.GUID
		};

	}

}
