using PMDDesktop.Requests.Users;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers;

internal static class GenericResponseCreators
{

	internal static UserInfoResponse CreateUserInfoResponse(User fromPOV, User ofUser)
	{

		bool canSeeLoginInfo = fromPOV.HasPermission(UserPermission.MANAGE_USERS);

		return new()
		{

			GUID = ofUser.GUID,
			DisplayName = ofUser.Name,
			HasPassword = canSeeLoginInfo ? ofUser.HasPassword() : null,
			LoginHandle = canSeeLoginInfo ? ofUser.LoginHandle : null

		};

	}

}
