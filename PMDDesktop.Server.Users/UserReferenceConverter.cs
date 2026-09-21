using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users;


internal class UserReferenceConverter : JsonConverter<UserReference>
{
	public override UserReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		string guidString = reader.GetString() ?? throw new JsonException($"We read a null string while trying to deserialize {Type}.");

		Guid guid = Guid.Parse(guidString);

		return new UserReference(guid);

	}

	public override void Write(Utf8JsonWriter writer, UserReference value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.GUID.ToString());

	}
}
