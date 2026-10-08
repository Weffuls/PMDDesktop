using PMDDesktop.Requests.Roles;
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

	internal static RoleInfoResponse CreateRoleInfoResponse(User fromPOV, UserRole ofRole)
	{

		bool canSeePermissionInfo = fromPOV.HasPermission(UserPermission.MANAGE_USERS);

		Dictionary<string, bool?>? permissions;

		if (canSeePermissionInfo)
		{

			permissions = [];
			foreach (UserPermission permission in UserPermission.ALL)
				permissions.Add(permission.DataName, ofRole.GetPermission(permission));

		}
		else
		{

			permissions = null;

		}

		return new()
		{

			DisplayName = ofRole.Name,
			GUID = ofRole.GUID,
			Permissions = permissions,
			IndexInRoleOrder = ofRole.GetCurrentOrderIndex(),
			IsDefaultRole = ofRole.IsDefaultRole

		};

	}

}
