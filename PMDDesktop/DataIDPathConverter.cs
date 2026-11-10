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

		MemoryExtensions.SpanSplitEnumerator<byte> parts = reader.ValueSpan.Split(DataIDPath.SPLIT_CHAR);
		Queue<DataID> datas = [];

		while (parts.MoveNext())
		{
			ReadOnlySpan<byte> bytes = reader.ValueSpan[parts.Current];

			if (!DataID.CanBeConverted(bytes, out string? failReason))
				throw new JsonException(failReason);

			datas.Enqueue(new(bytes));

		}

		return new DataIDPath(datas);

	}

	public override void Write(Utf8JsonWriter writer, DataIDPath value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.ToString());

	}

}
