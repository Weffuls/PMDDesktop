using PMDDesktop.Requests.Roles;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Roles;

public class GetRoleInfoRequestHandler : JsonRequestHandler<GetRoleInfoRequest, RoleInfoResponse>
{

	protected override async Task<ResponseResult<RoleInfoResponse, ResponseResultError>> CreateResponse(GetRoleInfoRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error1))
			return new(error1);

		if (!CommonResponseHandlerChecks.TryGetRole(game, request.GUID, out UserRole? role, out ResponseResultError? error2))
			return new(error2);

		return new(GenericResponseCreators.CreateRoleInfoResponse(requestingUser, role));

	}

}
