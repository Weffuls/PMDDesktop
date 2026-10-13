using System.Text.Json;

namespace PMDDesktop.Server.Users.Tests;

public class UserConverterTests
{

	public static readonly UserCreationOptions DEFAULT_USER_OPTIONS = new()
	{
		DisplayName = "Test User",
		LoginHandle = "test",
		PlainTextPassword = "Test123"
	};

	[Fact]
	public async Task ToAndFromJsonKeepsName()
	{

		UserManager manager1 = new();
		User? user1 = await manager1.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user1);

		using MemoryStream jsonStream = new();

		await JsonSerializer.SerializeAsync(jsonStream, user1, AppInfo.JSON_OPTIONS);

		jsonStream.Position = 0; // Rewind the stream so it can be read.

		UserManager manager2 = new();
		User user2 = await manager2.LoadAndAddUserJson(jsonStream, user1.GUID);

		Assert.Equal(user1.GUID, user2.GUID);
		Assert.Equal(user1.Name, user2.Name);
		Assert.Equal(user1.HashedPassword, user2.HashedPassword);
		Assert.Equal(user1.LoginHandle, user2.LoginHandle);

	}

	[Fact]
	public async Task ToAndFromJsonReattachesTokens()
	{

		UserManager manager1 = new();
		User? user1 = await manager1.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user1);

		for (int i = 0; i < 100; ++i) // Creating an excess so we make sure that discarded tokens are also treated correctly.
			await user1.CreateAccessToken();

		Assert.NotEmpty(manager1.accessTokens);
		Assert.NotEmpty(user1.accessTokens);

		using MemoryStream jsonStream = new();

		await JsonSerializer.SerializeAsync(jsonStream, user1, AppInfo.JSON_OPTIONS);

		jsonStream.Position = 0; // Rewind the stream so it can be read.

		UserManager manager2 = new();
		User user2 = await manager2.LoadAndAddUserJson(jsonStream, user1.GUID);

		Assert.NotEmpty(manager2.accessTokens);
		Assert.NotEmpty(user2.accessTokens);

		Assert.All(manager1.accessTokens.Values, originalToken =>
		{

			Assert.True(manager2.TryUseAccessToken(originalToken.TokenString, out User? user, out UserAccessToken? token));

			Assert.Equal(user2, user);
			Assert.Equal(originalToken.ExpiryDate, token.ExpiryDate);
			Assert.Equal(originalToken.TokenString, token.TokenString);

		});

		Assert.Equal(manager1.accessTokens.Count, manager2.accessTokens.Count);

	}

}
