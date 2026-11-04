using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop;

internal class DataIDConverter : JsonConverter<DataID>
{

	public override DataID? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		string readString = reader.GetString() ?? throw new JsonException($"We read a null string while trying to deserialize {Type}.");

		if (!DataID.CanBeConverted(readString, out string? reason))
			throw new JsonException(reason);

		return new DataID(readString);

	}

	public override void Write(Utf8JsonWriter writer, DataID value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.ToString());

	}

}
