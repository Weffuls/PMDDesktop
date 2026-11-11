using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop;

internal class DataIDPathConverter : JsonConverter<DataIDPath>
{

	public override DataIDPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		try
		{
			return (DataIDPath)reader.ValueSpan;
		}
		catch (InvalidCastException e)
		{
			throw new JsonException($"Unable to cast {Encoding.UTF8.GetString(reader.ValueSpan)} to {nameof(DataIDPath)}.", e);
		}

	}

	public override void Write(Utf8JsonWriter writer, DataIDPath value, JsonSerializerOptions options)
	{

		writer.WriteStringValue((string)value);

	}

}
