using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class CreateUserRequestHandler : JsonRequestHandler<CreateUserRequest, UserInfoResponse>
{

	protected override async Task<ResponseResult<UserInfoResponse, ResponseResultError>> CreateResponse(CreateUserRequest request, GameServer game, User? requestingUser)
	{

		if (!CommonResponseHandlerChecks.TryAuthorizeUser(requestingUser, [UserPermission.MANAGE_USERS], out ResponseResultError? error))
			return new(error);

		if (request.NewLoginHandle is not null && game.State.Users.TryGetUserByLoginHandle(request.NewLoginHandle, out _))
			return new(new ResponseResultDataCollisionError(nameof(request.NewLoginHandle), request.NewLoginHandle.ToString()));

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
