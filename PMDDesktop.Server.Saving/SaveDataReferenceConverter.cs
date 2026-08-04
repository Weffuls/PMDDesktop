using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving;

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
