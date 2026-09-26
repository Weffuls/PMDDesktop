using System.Text.Json;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Users.Tests.Roles;

public class UserRoleConverterTests
{

	[Fact]
	public async Task RoleJsonContainsNullsForUnspecified()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_USERS, true);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		JsonElement manageRolesElement = document.RootElement.GetProperty(UserRoleConverter.PERMISSIONS_PROPERTY).GetProperty(UserPermission.MANAGE_ROLES.DataName);

		Assert.Equal(JsonValueKind.Null, manageRolesElement.ValueKind);

	}

	[Fact]
	public async Task RoleJsonContainsTrueWhenSpecified()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_USERS, true);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		JsonElement manageRolesElement = document.RootElement.GetProperty(UserRoleConverter.PERMISSIONS_PROPERTY).GetProperty(UserPermission.MANAGE_USERS.DataName);

		Assert.Equal(JsonValueKind.True, manageRolesElement.ValueKind);

	}

	[Fact]
	public async Task RoleJsonContainsFalseWhenSpecified()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_USERS, false);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		JsonElement manageRolesElement = document.RootElement.GetProperty(UserRoleConverter.PERMISSIONS_PROPERTY).GetProperty(UserPermission.MANAGE_USERS.DataName);

		Assert.Equal(JsonValueKind.False, manageRolesElement.ValueKind);

	}

	[Fact]
	public async Task RoleJsonWithSpecifiedPermissionThenUnspecifiedContainsNull()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_USERS, true);
		await role.SetPermission(UserPermission.MANAGE_USERS, null);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		JsonElement manageRolesElement = document.RootElement.GetProperty(UserRoleConverter.PERMISSIONS_PROPERTY).GetProperty(UserPermission.MANAGE_USERS.DataName);

		Assert.Equal(JsonValueKind.Null, manageRolesElement.ValueKind);

	}

	[Fact]
	public async Task RoundTripRoleWithSpecifiedPermissionKeepsTrue()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_ROLES, true);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		UserRole? newRole = JsonSerializer.Deserialize<UserRole>(document);
		Assert.NotNull(newRole);

		Assert.Equal(true, newRole.GetPermission(UserPermission.MANAGE_ROLES));

	}

	[Fact]
	public async Task RoundTripRoleWithSpecifiedPermissionKeepsFalse()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_ROLES, false);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		UserRole? newRole = JsonSerializer.Deserialize<UserRole>(document);
		Assert.NotNull(newRole);

		Assert.Equal(false, newRole.GetPermission(UserPermission.MANAGE_ROLES));

	}

	[Fact]
	public async Task RoundTripRoleWithSpecifiedPermissionKeepsNull()
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_USERS, null);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		UserRole? newRole = JsonSerializer.Deserialize<UserRole>(document);
		Assert.NotNull(newRole);

		Assert.Null(newRole.GetPermission(UserPermission.MANAGE_USERS));
		Assert.Null(newRole.GetPermission(UserPermission.MANAGE_ROLES));

	}

}
