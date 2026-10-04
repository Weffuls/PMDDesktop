using PMDDesktop.Server.Users.Roles;
using System.Text.Json;

namespace PMDDesktop.Server.Users.Tests.Roles;

public class RoleReferenceTests
{

	[Fact]
	public async Task CanGetReference()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;
		UserRole? role = await roleManager.TryCreateRole();
		Assert.NotNull(role);

		RoleReference reference = new(role);

		Assert.True(reference.TryGetRole(manager, out UserRole? referRole));

		Assert.Equal(role, referRole);

	}

	[Fact]
	public async Task SerializationIsSimple()
	{

		UserManager manager = new();
		UserRoleManager roleManager = manager.RoleManager;
		UserRole? role = await roleManager.TryCreateRole();
		Assert.NotNull(role);

		RoleReference preReference = new(role);

		JsonElement json = JsonSerializer.SerializeToElement(preReference, AppInfo.JSON_OPTIONS);

		Assert.Equal(JsonValueKind.String, json.ValueKind);

		RoleReference? postReference = JsonSerializer.Deserialize<RoleReference>(json);

		Assert.NotNull(postReference);

		Assert.True(postReference.TryGetRole(manager, out UserRole? referRole));

		Assert.Equal(role, referRole);

	}

}
