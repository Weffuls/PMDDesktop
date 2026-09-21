using PMDDesktop.Requests.Auth;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Auth;

public class LoginRequestHandler : JsonRequestHandler<LoginRequest, LoginResponse>
{

	protected override async Task<LoginResponse> CreateResponse(LoginRequest request, GameServer game)
	{

		if (!game.State.Users.TryGetUserByLoginHandle(request.Handle, out User? user))
			throw new UserRequestException($"Invalid username or password.");

		if (!await user.VerifyPassword(request.Password))
			throw new UserRequestException($"Invalid username or password."); // Throw the same thing to be more "secure" I guess.

		return new()
		{
			AuthorizationToken = (await user.CreateAccessToken()).TokenString
		};

	}

}
