using System.Text.Json;

namespace PMDDesktop.Server.Users.Tests;

public class UserReferenceTests
{

	public static readonly UserCreationOptions DEFAULT_USER_OPTIONS = new()
	{
		DisplayName = "Test User",
		LoginHandle = "test",
		PlainTextPassword = "Test123"
	};

	[Fact]
	public async Task CanGetReference()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		UserReference reference = new(user);

		reference.TryGetUser(manager, out User? referUser);

		Assert.NotNull(referUser);
		Assert.Equal(referUser, user);

	}

	[Fact]
	public async Task SerializationIsSimple()
	{

		UserManager manager = new();
		User? user = await manager.TryCreateUser(DEFAULT_USER_OPTIONS);
		Assert.NotNull(user);

		UserReference preReference = new(user);

		JsonElement json = JsonSerializer.SerializeToElement(preReference, AppInfo.JSON_OPTIONS);

		Assert.Equal(JsonValueKind.String, json.ValueKind);

		UserReference? postReference = JsonSerializer.Deserialize<UserReference>(json);

		Assert.NotNull(postReference);

		postReference.TryGetUser(manager, out User? referUser);

		Assert.NotNull(referUser);
		Assert.Equal(referUser, user);

	}

}
