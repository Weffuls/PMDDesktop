using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class SetUserNameRequestHandler : JsonRequestHandler<SetUserNameRequest, UserInfoResponse>
{

	protected override async Task<ResponseResult<UserInfoResponse, ResponseResultError>> CreateResponse(SetUserNameRequest request, GameServer game, User? requestingUser)
	{

		// Check that they're logged in. We don't know what permissions they need yet.
		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error1))
			return new(error1);

		if (!CommonResponseHandlerChecks.TryGetUser(game, request.GUID, out User? targetUser, out ResponseResultError? error2))
			return new(error2);

		// To change the username of someone who isn't yourself, you need the MANAGE_USERS permission.
		if (targetUser != requestingUser)
		{

			// Do they have MANAGE_USERS?
			if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error3))
				return new(error3);

			// Are they higher in the Hierarchy?
			if (!CommonResponseHandlerChecks.TryCompareHierarchy(requestingUser, targetUser, out ResponseResultError? error4))
				return new(error4);

		}

		await targetUser.SetName(request.NewName);

		return new(GenericResponseCreators.CreateUserInfoResponse(requestingUser, targetUser));

	}

}
