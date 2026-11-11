using System.Text.Json;
using PMDDesktop;

namespace PMDDesktop.Tests;

public class DataIDPathConverterTests
{

	[Theory]
	[InlineData([new string[] { "hello", "node2", "the-end" }])]
	[InlineData([new string[] { "le-awesome", "sauce" }])]
	[InlineData([new string[] { "ice-cream" }])]
	public void ToAndBackJsonConversions(string[] input)
	{

		DataIDPath original = new(input.Select(inp => (DataID)inp));

		string jsonData = JsonSerializer.Serialize(original, AppInfo.JSON_OPTIONS);

		DataIDPath? deserialized = JsonSerializer.Deserialize<DataIDPath>(jsonData, AppInfo.JSON_OPTIONS);

		Assert.NotNull(deserialized);

		Assert.Equal(original, deserialized);
		Assert.True(original == deserialized);
		Assert.Equal(original.GetHashCode(), deserialized.GetHashCode());

	}

	[Theory]
	[InlineData("\"rabbits/are/neat\"", new string[] { "rabbits", "are", "neat" })]
	[InlineData("\"cool-beans\"", new string[] { "cool-beans" })]
	public void ConversionExpectations(string expectation, string[] inputs)
	{

		DataIDPath original = new(inputs.Select(inp => new DataID((DataID)inp)));

		string jsonData = JsonSerializer.Serialize(original, AppInfo.JSON_OPTIONS);

		Assert.Equal(expectation, jsonData);

	}

	[Theory]
	[InlineData("\"awesome sauce\"")]
	[InlineData("\"really//silly\"")]
	[InlineData("\"/\"")]
	[InlineData("\"/neat\"")]
	[InlineData("\"cool/\"")]
	[InlineData("\"/awesome/\"")]
	[InlineData("\"\"")]
	[InlineData("\"good/huh /good\"")]
	[InlineData("\"good/good/ huh\"")]
	public void ConversionErrors(string jsonData)
	{

		Assert.ThrowsAny<JsonException>(() =>
		{

			DataIDPath? path = JsonSerializer.Deserialize<DataIDPath>(jsonData, AppInfo.JSON_OPTIONS);

		});

	}

}
