using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

internal class AssetReferenceConverterFactory : JsonConverterFactory
{

	public override bool CanConvert(Type typeToConvert)
	{

		if (!typeToConvert.IsGenericType)
			return false;

		Type typeDefintion = typeToConvert.GetGenericTypeDefinition();

		return typeDefintion == typeof(AssetReference<>);

	}

	public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
	{

		Type genericType = typeToConvert.GetGenericArguments()[0];

		object? instance = Activator.CreateInstance(typeof(AssetReferenceConverter<>).MakeGenericType(genericType));

		if (instance is JsonConverter converter)
			return converter;

		throw new JsonException($"{instance} was not a JsonConverter. :(");

	}

}
