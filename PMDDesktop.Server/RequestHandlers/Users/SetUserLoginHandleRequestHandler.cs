using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class SetUserLoginHandleRequestHandler : JsonRequestHandler<SetUserLoginHandleRequest, UserInfoResponse>
{

	protected override async Task<UserInfoResponse> CreateResponse(SetUserLoginHandleRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		if (!game.TryGetUser(request.GUID, out User? user))
			throw new UserRequestException($"{request.GUID} does not match a user.");

		// To change the login handle of someone who isn't yourself, you need the MANAGE_USERS permission.
		if (user != requestingUser)
			if (!requestingUser.HasPermission(UserPermission.MANAGE_USERS))
				throw new UserRequestException($"You need permission to manage users to access this endpoint.");

		// TEMPORARY SOLUTION
		// TODO: Replace this with a more elegant solution. I think we should make a new class or struct to hold these types of super normalized strings.
		bool validHandle = request.NewLoginHandle is null || request.NewLoginHandle.All(letter => char.IsAsciiLetterLower(letter) || char.IsAsciiDigit(letter) || letter == '-');

		if (!validHandle)
			throw new UserRequestException($"The handle is invalid. Please make sure you use lowercase letters, numbers, or hyphens.");

		if (!await user.TrySetLoginHandle(request.NewLoginHandle))
			throw new UserRequestException($"Unable to set the login handle to '{request.NewLoginHandle}'. It may not be unique.");

		return GenericResponseCreators.CreateUserInfoResponse(requestingUser, user);

	}

}
