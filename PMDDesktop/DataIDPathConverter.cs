using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop;

internal class DataIDPathConverter : JsonConverter<DataIDPath>
{

	public override DataIDPath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		string readString = reader.GetString() ?? throw new JsonException($"We read a null string while trying to deserialize {Type}.");

		IEnumerable<DataID> nodes = readString.Split(DataIDPath.SPLIT_CHAR).Select(part =>
		{

			if (!DataID.CanBeConverted(part, out string? reason))
				throw new JsonException(reason);

			return new DataID(part);

		});

		return new DataIDPath(nodes);

	}

	public override void Write(Utf8JsonWriter writer, DataIDPath value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.ToString());

	}

}
