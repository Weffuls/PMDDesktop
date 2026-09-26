using PMDDesktop.Server.Users.Roles;
using System.Text.Json;

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

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	[InlineData(null)]
	public async Task RoundTripRoleWithSpecifiedPermissionKeepsValue(bool? value)
	{

		UserRole role = new();

		await role.SetPermission(UserPermission.MANAGE_USERS, value);

		JsonDocument document = JsonSerializer.SerializeToDocument(role);

		UserRole? newRole = JsonSerializer.Deserialize<UserRole>(document);
		Assert.NotNull(newRole);

		Assert.Equal(value, newRole.GetPermission(UserPermission.MANAGE_USERS));

	}

}
