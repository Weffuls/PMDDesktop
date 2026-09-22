using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users;


internal class UserConverter : JsonConverter<User>
{

	public static readonly string TOKENS_PROPERTY_NAME = "Tokens";
	public static readonly string CONFIG_PROPERTY_NAME = "Config";
	public static readonly string TOKEN_STRING_PROPERTY_NAME = "String";
	public static readonly string TOKEN_EXPIRE_PROPERTY_NAME = "ExpiryDate";

	public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		using JsonDocument json = JsonDocument.ParseValue(ref reader);

		List<string> propertyList = [.. json.RootElement.EnumerateObject().Select(element => element.Name)];

		User user = new()
		{
			Name = QuickReadString(json, propertyList, nameof(User.Name)),
			HashedPassword = QuickReadNullableString(json, propertyList, nameof(User.HashedPassword)),
			LoginHandle = QuickReadNullableString(json, propertyList, nameof(User.LoginHandle))
		};

		propertyList.Remove(TOKENS_PROPERTY_NAME);
		user.accessTokens = ReadTokenArray(json, user);

		if (propertyList.Count > 0)
			throw new JsonException($"The user json had left-over properties after parsing, such as {propertyList[0]}. These unused properties cannot be accounted for, and may result in data loss. Is it from a newer version?");

		if (user.IsAlive())
			throw new InvalidOperationException($"{user} should not be attached to a manager at this point, or otherwise \"alive\".");

		return user;

	}

	public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
	{

		writer.WriteStartObject();

		writer.WriteString(nameof(User.Name), value.Name);
		writer.WriteString(nameof(User.HashedPassword), value.HashedPassword);
		writer.WriteString(nameof(User.LoginHandle), value.LoginHandle);

		writer.WriteStartArray(TOKENS_PROPERTY_NAME);
		foreach (UserAccessToken token in value.accessTokens)
			WriteToken(writer, token);
		writer.WriteEndArray();

		writer.WriteEndObject();

	}

	private static void WriteToken(Utf8JsonWriter writer, UserAccessToken token)
	{

		writer.WriteStartObject();

		writer.WriteString(TOKEN_STRING_PROPERTY_NAME, token.TokenString);
		writer.WriteString(TOKEN_EXPIRE_PROPERTY_NAME, token.ExpiryDate);

		writer.WriteEndObject();

	}

	private static string QuickReadString(JsonDocument document, List<string> propertyList, string name)
	{

		propertyList.Remove(name);

		return document.RootElement.GetProperty(name).GetString()
			?? throw new JsonException($"{name} should not be null in {document}");

	}

	private static string? QuickReadNullableString(JsonDocument document, List<string> propertyList, string name)
	{

		propertyList.Remove(name);

		return document.RootElement.GetProperty(name).GetString();

	}

	private static List<UserAccessToken> ReadTokenArray(JsonDocument document, User user)
	{

		List<UserAccessToken> tokens = [];

		JsonElement tokenJsonArray = document.RootElement.GetProperty(TOKENS_PROPERTY_NAME);

		foreach (JsonElement token in tokenJsonArray.EnumerateArray())
		{

			string tokenString = token.GetProperty(TOKEN_STRING_PROPERTY_NAME).GetString()
				?? throw new JsonException($"{TOKEN_STRING_PROPERTY_NAME} should not be null in {token}");

			DateTime expiryDate = token.GetProperty(TOKEN_EXPIRE_PROPERTY_NAME).GetDateTime();

			tokens.Add(new(tokenString, user, expiryDate));

		}

		return tokens;

	}

}
