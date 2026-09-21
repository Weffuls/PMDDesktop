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
		Assert.True(await manager.TryCreateUser(DEFAULT_USER_OPTIONS));
		
		string? loginHandle = DEFAULT_USER_OPTIONS.LoginHandle;
		Assert.NotNull(loginHandle);
		Assert.True(manager.TryGetUserByLoginHandle(loginHandle, out User? user));

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
		Assert.True(await manager.TryCreateUser(DEFAULT_USER_OPTIONS with
		{
			PlainTextPassword = null
		}));

		string? loginHandle = DEFAULT_USER_OPTIONS.LoginHandle;
		Assert.NotNull(loginHandle);
		Assert.True(manager.TryGetUserByLoginHandle(loginHandle, out User? user));

		string? defaultPassword = DEFAULT_USER_OPTIONS.PlainTextPassword;
		Assert.NotNull(defaultPassword);

		Assert.False(await user.VerifyPassword(defaultPassword));
		Assert.False(await user.VerifyPassword("Test456"));

	}

	[Fact]
	public async Task AccessTokenGrantsAccess()
	{
		
		UserManager manager = new();
		Assert.True(await manager.TryCreateUser(DEFAULT_USER_OPTIONS));

		string? loginHandle = DEFAULT_USER_OPTIONS.LoginHandle;
		Assert.NotNull(loginHandle);
		Assert.True(manager.TryGetUserByLoginHandle(loginHandle, out User? user));

		UserAccessToken token = await user.CreateAccessToken();

		Assert.True(manager.TryUseAccessToken(token.TokenString, out User? outUser));

		Assert.Equal(user, outUser);

	}

	[Fact]
	public async Task LoginHandleChanges()
	{
		
		UserManager manager = new();
		Assert.True(await manager.TryCreateUser(DEFAULT_USER_OPTIONS));

		string? loginHandle = DEFAULT_USER_OPTIONS.LoginHandle;
		Assert.NotNull(loginHandle);
		Assert.True(manager.TryGetUserByLoginHandle(loginHandle, out User? user));

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
		Assert.True(await manager.TryCreateUser(DEFAULT_USER_OPTIONS));

		string? loginHandle = DEFAULT_USER_OPTIONS.LoginHandle;
		Assert.NotNull(loginHandle);
		Assert.True(manager.TryGetUserByLoginHandle(loginHandle, out User? user));

		Guid guid = user.GUID;

		Assert.True(manager.TryGetUser(guid, out User? guidUser));
		Assert.NotNull(guidUser);

		Assert.False(manager.TryGetUser(Guid.Empty, out User? notGuidUser));
		Assert.Null(notGuidUser);

	}

}
