using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Users;

public class SetUserNameRequestHandler : JsonRequestHandler<SetUserNameRequest, UserInfoResponse>
{

	protected override async Task<UserInfoResponse> CreateResponse(SetUserNameRequest request, GameServer game, User? requestingUser)
	{

		if (requestingUser is null)
			throw new UserRequestException($"You need to be logged in to access this endpoint.");

		if (!game.TryGetUser(request.GUID, out User? user))
			throw new UserRequestException($"{request.GUID} does not match a user.");

		// To change the username of someone who isn't yourself, you need the MANAGE_USERS permission.
		if (user != requestingUser)
		{
			if (!requestingUser.HasPermission(UserPermission.MANAGE_USERS))
				throw new UserRequestException($"You need permission to manage users to access this endpoint.");
			if (!requestingUser.HigherThan(user))
				throw new UserRequestException($"You are not high enough in the role hierarchy to perform this operation.");
		}

		await user.SetName(request.NewName);

		return GenericResponseCreators.CreateUserInfoResponse(requestingUser, user);

	}

}
