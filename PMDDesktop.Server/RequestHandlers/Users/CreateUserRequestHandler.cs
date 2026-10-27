using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class CreateUserRequestHandler : JsonRequestHandler<CreateUserRequest, UserInfoResponse>
{

	protected override async Task<ResponseJsonResult<UserInfoResponse, ResponseResultError>> CreateResponse(CreateUserRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [UserPermission.MANAGE_USERS], out ResponseResultError? error))
			return new(error);

		// TEMPORARY SOLUTION
		// TODO: Replace this with a more elegant solution. I think we should make a new class or struct to hold these types of super normalized strings.
		bool validHandle = request.NewLoginHandle is null || request.NewLoginHandle.All(letter => char.IsAsciiLetterLower(letter) || char.IsAsciiDigit(letter) || letter == '-');
		if (!validHandle)
			return new(new ResponseResultDataInvalidError(nameof(request.NewLoginHandle), request.NewLoginHandle));

		if (request.NewLoginHandle is not null && game.State.Users.TryGetUserByLoginHandle(request.NewLoginHandle, out _))
			return new(new ResponseResultDataCollisionError(nameof(request.NewLoginHandle), request.NewLoginHandle));

		UserCreationOptions options = new()
		{
			DisplayName = request.NewName,
			LoginHandle = request.NewLoginHandle,
			PlainTextPassword = request.NewPassword
		};

		User user = await game.State.Users.TryCreateUser(options)
			?? throw new Exception($"Unable to create user.");

		return new(GenericResponseCreators.CreateUserInfoResponse(requestingUser, user));

	}

}
