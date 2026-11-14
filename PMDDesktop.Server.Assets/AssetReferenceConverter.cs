using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

internal class AssetReferenceConverter<T> : JsonConverter<AssetReference<T>> where T : Asset
{
	public override AssetReference<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize. Instead got {reader.TokenType}");

		return new AssetReference<T>((DataIDPath)reader.ValueSpan);

	}

	public override void Write(Utf8JsonWriter writer, AssetReference<T> value, JsonSerializerOptions options)
	{

		writer.WriteStringValue((string)value.Location);

	}
}
