using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// <see cref="JsonConverter{SaveDataReference{SaveData}}"/> to convert <see cref="SaveDataReference{SaveData}"/> into a simple string of the referenced <see cref="Guid"/>.
/// </summary>
/// <typeparam name="T">The Type of <see cref="SaveData"/> referenced.</typeparam>
internal class SaveDataReferenceConverter<T> : JsonConverter<SaveDataReference<T>> where T : SaveData
{
	public override SaveDataReference<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		if (reader.TokenType != JsonTokenType.String)
			throw new JsonException($"{Type} was expecting a string token to deserialize.");

		string guidString = reader.GetString() ?? throw new JsonException($"We read a null string while trying to deserialize {Type}.");

		Guid guid = Guid.Parse(guidString);

		return new SaveDataReference<T>(guid);

	}

	public override void Write(Utf8JsonWriter writer, SaveDataReference<T> value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.GUID.ToString());

	}
}
