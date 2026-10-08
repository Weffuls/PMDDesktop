using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class CreateUserRequestHandler : JsonRequestHandler<CreateUserRequest, UserInfoResponse>
{

	protected override async Task<UserInfoResponse> CreateResponse(CreateUserRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		// To create users, you need the MANAGE_USERS permission.
		if (!requestingUser.HasPermission(UserPermission.MANAGE_USERS))
			throw new UserRequestException($"You need permission to manage users to access this endpoint.");

		// TEMPORARY SOLUTION
		// TODO: Replace this with a more elegant solution. I think we should make a new class or struct to hold these types of super normalized strings.
		bool validHandle = request.NewLoginHandle is null || request.NewLoginHandle.All(letter => char.IsAsciiLetterLower(letter) || char.IsAsciiDigit(letter) || letter == '-');

		if (!validHandle)
			throw new UserRequestException($"The handle is invalid. Please make sure you use lowercase letters, numbers, or hyphens.");

		if (request.NewLoginHandle is not null && game.State.Users.TryGetUserByLoginHandle(request.NewLoginHandle, out _))
			throw new UserRequestException($"Provided login handle '{request.NewLoginHandle}' is already in use.");

		UserCreationOptions options = new()
		{
			DisplayName = request.NewName,
			LoginHandle = request.NewLoginHandle,
			PlainTextPassword = request.NewPassword
		};

		User user = await game.State.Users.TryCreateUser(options)
			?? throw new Exception($"Unable to create user.");

		return GenericResponseCreators.CreateUserInfoResponse(requestingUser, user);

	}

}
