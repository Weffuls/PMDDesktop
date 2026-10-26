using PMDDesktop.Requests.Auth;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Auth;

public class WhoAmIRequestHandler : JsonRequestHandler<WhoAmIRequest, WhoAmIResponse>
{

	protected override async Task<ResponseJsonResult<WhoAmIResponse, ResponseResultError>> CreateResponse(WhoAmIRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error))
			return new(error);

		return new(new WhoAmIResponse()
		{
			GUID = requestingUser.GUID
		});

	}

}
