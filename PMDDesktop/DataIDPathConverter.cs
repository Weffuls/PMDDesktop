using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop;

internal class DataIDPathConverter : JsonConverter<DataIDPath>
{

	public override DataIDPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		return (DataIDPath)reader.ValueSpan;

	}

	public override void Write(Utf8JsonWriter writer, DataIDPath value, JsonSerializerOptions options)
	{

		writer.WriteStringValue((string)value);

	}

}
