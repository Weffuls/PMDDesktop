using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class SetUserLoginHandleRequestHandler : JsonRequestHandler<SetUserLoginHandleRequest, UserInfoResponse>
{

	protected override async Task<ResponseResult<UserInfoResponse, ResponseResultError>> CreateResponse(SetUserLoginHandleRequest request, GameServer game, User? requestingUser)
	{

		// Check that they're logged in. We don't know what permissions they need yet.
		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error1))
			return new(error1);

		if (!CommonResponseHandlerChecks.TryGetUser(game, request.GUID, out User? targetUser, out ResponseResultError? error2))
			return new(error2);

		// To change the login handle of someone who isn't yourself, you need the MANAGE_USERS permission.
		if (targetUser != requestingUser)
		{

			// Do they have MANAGE_USERS?
			if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [], out ResponseResultError? error3))
				return new(error3);

			// Are they higher in the Hierarchy?
			if (!CommonResponseHandlerChecks.TryCompareHierarchy(requestingUser, targetUser, out ResponseResultError? error4))
				return new(error4);

		}

		// TEMPORARY SOLUTION
		// TODO: Replace this with a more elegant solution. I think we should make a new class or struct to hold these types of super normalized strings.
		bool validHandle = request.NewLoginHandle is null || request.NewLoginHandle.All(letter => char.IsAsciiLetterLower(letter) || char.IsAsciiDigit(letter) || letter == '-');
		if (!validHandle)
			return new(new ResponseResultDataInvalidError(nameof(request.NewLoginHandle), request.NewLoginHandle));

		// Check to make sure the new login handle is unique.
		if (request.NewLoginHandle is not null && game.State.Users.TryGetUserByLoginHandle(request.NewLoginHandle, out _))
			return new(new ResponseResultDataCollisionError(nameof(request.NewLoginHandle), request.NewLoginHandle));

		if (!await targetUser.TrySetLoginHandle(request.NewLoginHandle))
			throw new Exception($"Unable to set the login handle to '{request.NewLoginHandle}'. It may not be unique.");

		return new(GenericResponseCreators.CreateUserInfoResponse(requestingUser, targetUser));

	}

}
