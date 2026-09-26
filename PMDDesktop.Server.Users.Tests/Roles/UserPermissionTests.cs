using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Users.Tests.Roles;

public class UserPermissionTests
{

	[Theory]
	[InlineData("manage-users")]
	[InlineData("manage-roles")]
	public void UserPermissionAllIncludesPermissions(string dataName)
	{

		Assert.Contains(UserPermission.ALL, permission => permission.DataName == dataName);

	}

}
