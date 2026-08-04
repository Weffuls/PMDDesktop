using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

internal class AssetReferenceConverter<T> : JsonConverter<AssetReference<T>> where T : Asset
{
	public override AssetReference<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		string locationString = reader.GetString() ?? throw new JsonException($"We read a null string while trying to deserialize {Type}.");

		AssetLocation location = new(locationString);

		return new AssetReference<T>(location);

	}

	public override void Write(Utf8JsonWriter writer, AssetReference<T> value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.Location.ToString());

	}
}
