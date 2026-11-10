using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop;

internal class DataIDConverter : JsonConverter<DataID>
{

	public override DataID? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		if (!DataID.CanBeConverted(reader.ValueSpan, out string? reason))
			throw new JsonException(reason);

		return new DataID(reader.ValueSpan);

	}

	public override void Write(Utf8JsonWriter writer, DataID value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.ToString());

	}

}
