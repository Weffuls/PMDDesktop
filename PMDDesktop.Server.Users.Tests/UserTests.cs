using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Users.Tests;

public class UserTests
{

	public static readonly UserCreationOptions DEFAULT_USER_OPTIONS = new()
	{
		DisplayName = "Test User",
		LoginHandle = "test",
		PlainTextPassword = "Test123"
	};

	[Fact]
	public async Task PasswordVerification()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		string? defaultPassword = DEFAULT_USER_OPTIONS.PlainTextPassword;
		Assert.NotNull(defaultPassword);

		Assert.False(await user.VerifyPassword("Test456"));
		Assert.False(await user.VerifyPassword(""));
		Assert.True(await user.VerifyPassword(defaultPassword));

	}

	[Fact]
	public async Task NullPasswordVerification()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS with
		{
			PlainTextPassword = null
		});
		Assert.NotNull(user);

		string? defaultPassword = DEFAULT_USER_OPTIONS.PlainTextPassword;
		Assert.NotNull(defaultPassword);

		Assert.False(await user.VerifyPassword(defaultPassword));
		Assert.False(await user.VerifyPassword("Test456"));

	}

	[Fact]
	public async Task AccessTokenGrantsAccess()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		UserAccessToken token = await user.CreateAccessToken();

		Assert.True(manager.TryUseAccessToken(token.TokenString, out User? outUser));

		Assert.Equal(user, outUser);

	}

	[Fact]
	public async Task LoginHandleChanges()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		string? loginHandle = DEFAULT_USER_OPTIONS.LoginHandle;
		Assert.NotNull(loginHandle);
		Assert.True(manager.TryGetUserByLoginHandle(loginHandle, out _));

		string newHandle = "new-handle";

		Assert.True(await user.TrySetLoginHandle(newHandle));

		Assert.False(manager.TryGetUserByLoginHandle(loginHandle, out User? oldHandleUser));
		Assert.Null(oldHandleUser);

		Assert.True(manager.TryGetUserByLoginHandle(newHandle, out User? newHandleUser));
		Assert.NotNull(newHandleUser);

	}

	[Fact]
	public async Task GetViaGUID()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		Guid guid = user.GUID;

		Assert.True(manager.TryGetUser(guid, out User? guidUser));
		Assert.NotNull(guidUser);

		Assert.False(manager.TryGetUser(Guid.Empty, out User? notGuidUser));
		Assert.Null(notGuidUser);

	}

	[Fact]
	public async Task IsTokenLimitEnforced()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		for (int i = 0; i < User.ACCESS_TOKEN_LIMIT + 100; ++i)
		{
			await user.CreateAccessToken();
		}

		Assert.Equal(manager.accessTokens.Count, User.ACCESS_TOKEN_LIMIT);

	}

	[Fact]
	public async Task AreOldestTokensRemovedOnLimit()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		for (int i = 0; i < User.ACCESS_TOKEN_LIMIT; ++i)
			await user.CreateAccessToken();

		Assert.Equal(manager.accessTokens.Count, User.ACCESS_TOKEN_LIMIT);

		List<UserAccessToken> seenTokens = [.. manager.accessTokens.Values];

		for (int i = 0; i < User.ACCESS_TOKEN_LIMIT; ++i)
			await user.CreateAccessToken();

		Assert.Equal(manager.accessTokens.Count, User.ACCESS_TOKEN_LIMIT);

		seenTokens.AddRange(manager.accessTokens.Values);
		Assert.Distinct(seenTokens);

	}

	[Fact]
	public async Task DeleteUserTest()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);
		await user.CreateAccessToken();

		Assert.True(manager.TryGetUser(user.GUID, out _));

		Assert.NotEmpty(manager.accessTokens);
		Assert.NotEmpty(manager.guids);
		Assert.NotEmpty(manager.loginHandles);

		Assert.True(user.IsAlive());

		Assert.True(await user.TryRemoveUser());

		Assert.False(manager.TryGetUser(user.GUID, out _));

		Assert.Empty(manager.accessTokens);
		Assert.Empty(manager.guids);
		Assert.Empty(manager.loginHandles);

		Assert.False(user.IsAlive());

	}

	[Fact]
	public async Task CanChangeName()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		string originalName = user.Name;

		await user.SetName("Waffles");

		Assert.Equal("Waffles", user.Name);

		Assert.NotEqual(originalName, user.Name);

	}

	// I'm not sure if there's a better place for these role/permission/user tests. They test way more than just the User.cs file, but they start and end there.
	[Fact]
	public async Task UserGetsPermission()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		UserRoleManager roleManager = manager.RoleManager;
		UserRole? role = await roleManager.TryCreateRole();
		Assert.NotNull(role);
		await role.SetPermission(UserPermission.MANAGE_ROLES, true);

		Assert.False(user.HasPermission(UserPermission.MANAGE_ROLES));

		Assert.True(await user.TryAddRole(role));

		Assert.True(user.HasPermission(UserPermission.MANAGE_ROLES));

		Assert.True(await user.TryRemoveRole(role));

		Assert.False(user.HasPermission(UserPermission.MANAGE_ROLES));

	}

	[Fact]
	public async Task UserPermissionsRespectHierarchy()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		UserRoleManager roleManager = manager.RoleManager;
		UserRole? givingRole = await roleManager.TryCreateRole();
		Assert.NotNull(givingRole);
		await givingRole.SetPermission(UserPermission.MANAGE_ROLES, true);
		await user.TryAddRole(givingRole);

		UserRole? takingRole = await roleManager.TryCreateRole();
		Assert.NotNull(takingRole);
		await takingRole.SetPermission(UserPermission.MANAGE_ROLES, false);
		await user.TryAddRole(takingRole);

		UserRole? nothingRole = await roleManager.TryCreateRole();
		Assert.NotNull(nothingRole);
		await nothingRole.SetPermission(UserPermission.MANAGE_ROLES, null);
		await user.TryAddRole(nothingRole);

		await roleManager.SetOrder([givingRole.GUID, takingRole.GUID, nothingRole.GUID]);
		Assert.True(user.HasPermission(UserPermission.MANAGE_ROLES));

		await roleManager.SetOrder([takingRole.GUID, givingRole.GUID, nothingRole.GUID]);
		Assert.False(user.HasPermission(UserPermission.MANAGE_ROLES));

		await roleManager.SetOrder([nothingRole.GUID, givingRole.GUID, takingRole.GUID]);
		Assert.True(user.HasPermission(UserPermission.MANAGE_ROLES));

		await roleManager.SetOrder([nothingRole.GUID, takingRole.GUID, givingRole.GUID]);
		Assert.False(user.HasPermission(UserPermission.MANAGE_ROLES));

	}

}
