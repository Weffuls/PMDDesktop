using PMDDesktop.Requests.Auth;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Auth;

public class LoginRequestHandler : JsonRequestHandler<LoginRequest, LoginResponse>
{

	protected override async Task<ResponseJsonResult<LoginResponse, ResponseResultError>> CreateResponse(LoginRequest request, GameServer game, User? requestingUser)
	{

		if (!game.State.Users.TryGetUserByLoginHandle(request.Handle, out User? user))
			return new(new ResponseResultLoginError());

		if (!await user.VerifyPassword(request.Password))
			return new(new ResponseResultLoginError());

		return new(new LoginResponse()
		{
			AuthorizationToken = (await user.CreateAccessToken()).TokenString
		});

	}

}
