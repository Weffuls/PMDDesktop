using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Users.Tests.Roles;

public class UserRoleTests
{

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	[InlineData(null)]
	public async Task SetPermissionsAndReturnExpectedPermissions(bool? value)
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_ROLES, value);

		Assert.Equal(value, role.GetPermission(UserPermission.MANAGE_ROLES));

	}

	[Fact]
	public async Task UnsetPermissionsAreNull()
	{

		UserRole role = new();

		Assert.All(UserPermission.ALL, permission => Assert.Null(role.GetPermission(permission)));

	}

}
