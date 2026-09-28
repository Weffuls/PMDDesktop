using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Users.Tests.Roles;

public class UserRoleManagerTests
{

	[Fact]
	public void UserRoleManagerPointsBackAtUserManager()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;

		Assert.NotNull(roleManager);

		Assert.Equal(manager, roleManager.Manager);

		Assert.Equal(roleManager, roleManager.Manager.RoleManager);

	}

	[Fact]
	public void UserRoleManagerHasDefaultRole()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;

		Assert.NotNull(roleManager.DefaultRole);

		Assert.True(roleManager.DefaultRole.IsDefaultRole);

		Assert.Equal(Guid.Empty, roleManager.DefaultRole.GUID);

		// Seeing if that's what we get for Guid.Empty

		Assert.True(roleManager.TryGetRole(Guid.Empty, out UserRole? tryEmptyRole));
		Assert.NotNull(tryEmptyRole);
		Assert.Equal(roleManager.DefaultRole, tryEmptyRole);

		// Test comparing to mock default role.

		UserRole exampleDefault = UserRole.CreateDefaultRole();
		UserRole managerDefault = roleManager.DefaultRole;

		Assert.All(UserPermission.ALL, perm => Assert.Equal(exampleDefault.GetPermission(perm), managerDefault.GetPermission(perm)));

	}

	[Fact]
	public async Task NewRolesAreOrderedLastBeforeDefault()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;

		// Role 0
		UserRole role0 = roleManager.DefaultRole;
		Assert.Equal(role0, roleManager.Skip(0).First());
		Assert.Single(roleManager);
		Assert.Equal(role0, roleManager.SkipLast(0).Last());

		// Role 1

		UserRole? role1 = await roleManager.TryCreateRole();
		Assert.Equal(role1, roleManager.Skip(0).First());
		Assert.Equal(role0, roleManager.Skip(1).First());
		Assert.Equal(2, roleManager.Count());
		Assert.Equal(role1, roleManager.SkipLast(1).Last());
		Assert.Equal(role0, roleManager.SkipLast(0).Last());

		// Role 2

		UserRole? role2 = await roleManager.TryCreateRole();
		Assert.Equal(role1, roleManager.Skip(0).First());
		Assert.Equal(role2, roleManager.Skip(1).First());
		Assert.Equal(role0, roleManager.Skip(2).First());
		Assert.Equal(3, roleManager.Count());
		Assert.Equal(role1, roleManager.SkipLast(2).Last());
		Assert.Equal(role2, roleManager.SkipLast(1).Last());
		Assert.Equal(role0, roleManager.SkipLast(0).Last());

		// Role 3

		UserRole? role3 = await roleManager.TryCreateRole();
		Assert.Equal(role1, roleManager.Skip(0).First());
		Assert.Equal(role2, roleManager.Skip(1).First());
		Assert.Equal(role3, roleManager.Skip(2).First());
		Assert.Equal(role0, roleManager.Skip(3).First());
		Assert.Equal(4, roleManager.Count());
		Assert.Equal(role1, roleManager.SkipLast(3).Last());
		Assert.Equal(role2, roleManager.SkipLast(2).Last());
		Assert.Equal(role3, roleManager.SkipLast(1).Last());
		Assert.Equal(role0, roleManager.SkipLast(0).Last());

		// Role 4

		UserRole? role4 = await roleManager.TryCreateRole();
		Assert.Equal(role1, roleManager.Skip(0).First());
		Assert.Equal(role2, roleManager.Skip(1).First());
		Assert.Equal(role3, roleManager.Skip(2).First());
		Assert.Equal(role4, roleManager.Skip(3).First());
		Assert.Equal(role0, roleManager.Skip(4).First());
		Assert.Equal(5, roleManager.Count());
		Assert.Equal(role1, roleManager.SkipLast(4).Last());
		Assert.Equal(role2, roleManager.SkipLast(3).Last());
		Assert.Equal(role3, roleManager.SkipLast(2).Last());
		Assert.Equal(role4, roleManager.SkipLast(1).Last());
		Assert.Equal(role0, roleManager.SkipLast(0).Last());

	}

	[Fact]
	public async Task SortingAffectsEnumeration()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;

		UserRole defaultRole = roleManager.DefaultRole;
		UserRole? role1 = await roleManager.TryCreateRole();
		UserRole? role2 = await roleManager.TryCreateRole();
		UserRole? role3 = await roleManager.TryCreateRole();
		UserRole? role4 = await roleManager.TryCreateRole();

		Assert.NotNull(role1);
		Assert.NotNull(role2);
		Assert.NotNull(role3);
		Assert.NotNull(role4);

		UserRole[] array1 = [.. roleManager];

		Assert.Equal(role1, array1[0]);
		Assert.Equal(role2, array1[1]);
		Assert.Equal(role3, array1[2]);
		Assert.Equal(role4, array1[3]);
		Assert.Equal(defaultRole, array1[4]);

		UserRole[] newSort2 = [role3, role2, role1, role4];

		await roleManager.SetOrder(newSort2.Select(role => role.GUID));

		UserRole[] array2 = [.. roleManager];

		Assert.Equal(newSort2[0], array2[0]);
		Assert.Equal(newSort2[1], array2[1]);
		Assert.Equal(newSort2[2], array2[2]);
		Assert.Equal(newSort2[3], array2[3]);
		Assert.Equal(defaultRole, array2[4]);

	}

	[Fact]
	public async Task SortingAffectsEnumerationWithDefaultSpecified()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;

		UserRole defaultRole = roleManager.DefaultRole;
		UserRole? role1 = await roleManager.TryCreateRole();
		UserRole? role2 = await roleManager.TryCreateRole();
		UserRole? role3 = await roleManager.TryCreateRole();
		UserRole? role4 = await roleManager.TryCreateRole();

		Assert.NotNull(role1);
		Assert.NotNull(role2);
		Assert.NotNull(role3);
		Assert.NotNull(role4);

		UserRole[] array1 = [.. roleManager];

		Assert.Equal(role1, array1[0]);
		Assert.Equal(role2, array1[1]);
		Assert.Equal(role3, array1[2]);
		Assert.Equal(role4, array1[3]);
		Assert.Equal(defaultRole, array1[4]);

		UserRole[] newSort2 = [role3, role2, role1, role4, defaultRole];

		await roleManager.SetOrder(newSort2.Select(role => role.GUID));

		UserRole[] array2 = [.. roleManager];

		Assert.Equal(newSort2[0], array2[0]);
		Assert.Equal(newSort2[1], array2[1]);
		Assert.Equal(newSort2[2], array2[2]);
		Assert.Equal(newSort2[3], array2[3]);
		Assert.Equal(newSort2[4], array2[4]);

	}

}
