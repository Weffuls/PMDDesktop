using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

/// <summary>
/// JsonConverter to convert SaveDataReference into a simple string of the referenced GUID.
/// </summary>
/// <typeparam name="T">The Type of SaveData referenced.</typeparam>
internal class SaveDataReferenceConverter<T> : JsonConverter<SaveDataReference<T>> where T : SaveData
{
	public override SaveDataReference<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{

		throw new NotImplementedException();

	}

	public override void Write(Utf8JsonWriter writer, SaveDataReference<T> value, JsonSerializerOptions options)
	{

		writer.WriteStringValue(value.GUID);

	}
}
