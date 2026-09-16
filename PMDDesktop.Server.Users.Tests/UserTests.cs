namespace PMDDesktop.Server.Users.Tests;

public class UserTests
{

	[Fact]
	public async Task PasswordVerification()
	{

		User user = new();

		await user.SetPassword("Test123");

		Assert.False(await user.VerifyPassword("Test456"));
		Assert.False(await user.VerifyPassword(""));
		Assert.True(await user.VerifyPassword("Test123"));

	}

	[Fact]
	public async Task NullPasswordVerification()
	{

		User user = new();

		Assert.False(await user.VerifyPassword("Test123"));
		Assert.False(await user.VerifyPassword("Test456"));

	}

}
