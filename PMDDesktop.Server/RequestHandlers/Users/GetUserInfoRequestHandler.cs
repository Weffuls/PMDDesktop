using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class GetUserInfoRequestHandler : JsonRequestHandler<GetUserInfoRequest, UserInfoResponse>
{

	protected override async Task<ResponseResult<UserInfoResponse, ResponseResultError>> CreateResponse(GetUserInfoRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error1))
			return new(error1);

		if (!CommonResponseHandlerChecks.TryGetUser(game, request.GUID, out User? user, out ResponseResultError? error2))
			return new(error2);

		return new(GenericResponseCreators.CreateUserInfoResponse(requestingUser, user));

	}

}
