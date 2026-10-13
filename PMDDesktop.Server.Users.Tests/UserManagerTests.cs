using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.Users.Tests;

public class UserManagerTests
{

	public static readonly UserCreationOptions DEFAULT_USER_OPTIONS = new()
	{
		DisplayName = "Test User",
		LoginHandle = "test",
		PlainTextPassword = "Test123"
	};

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
	public async Task DefaultAdminUserIsAdmin()
	{

		UserManager manager = new();

		User? user = await manager.TryCreateDefaultAdminUser();
		Assert.NotNull(user);

		Assert.True(user.IsAdmin);

	}

}
