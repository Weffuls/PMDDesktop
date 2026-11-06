using System.Text.Json;
using PMDDesktop;

namespace PMDDesktop.Tests;

public class DataIDPathConverterTests
{

	[Theory]
	[InlineData([new string[] { "Hello", "Node2", "TheEnd" }])]
	[InlineData([new string[] { "  le-awesome", "sauce  " }])]
	[InlineData([new string[] { "ice-cream" }])]
	public void ToAndBackJsonConversions(string[] input)
	{

		DataIDPath original = new(input.Select(inp => new DataID(inp)));

		string jsonData = JsonSerializer.Serialize(original, AppInfo.JSON_OPTIONS);

		DataIDPath? deserialized = JsonSerializer.Deserialize<DataIDPath>(jsonData, AppInfo.JSON_OPTIONS);

		Assert.NotNull(deserialized);

		Assert.Equal(original, deserialized);
		Assert.True(original == deserialized);
		Assert.Equal(original.GetHashCode(), deserialized.GetHashCode());

	}

	[Theory]
	[InlineData("\"cats/are/cool\"", "\"CATS   /Are/   COOL\"")]
	[InlineData("\"  awesome-sauce\"", "\"AWESOME-SAUCE \"")]
	public void EqualConversions(string string1, string string2)
	{

		DataIDPath? path1 = JsonSerializer.Deserialize<DataIDPath>(string1, AppInfo.JSON_OPTIONS);
		DataIDPath? path2 = JsonSerializer.Deserialize<DataIDPath>(string2, AppInfo.JSON_OPTIONS);

		Assert.NotNull(path1);
		Assert.NotNull(path2);

		Assert.Equal(path1, path2);
		Assert.True(path1 == path2);
		Assert.Equal(path1.GetHashCode(), path2.GetHashCode());

	}

	[Theory]
	[InlineData("\"rabbits/are/neat\"", new string[] { "rabbits", "are", "neat" })]
	[InlineData("\"cool-beans\"", new string[] { "cool-beans" })]
	public void ConversionExpectations(string expectation, string[] inputs)
	{

		DataIDPath original = new(inputs.Select(inp => new DataID(inp)));

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
	public void ConversionErrors(string jsonData)
	{

		Assert.ThrowsAny<JsonException>(() =>
		{

			DataIDPath? path = JsonSerializer.Deserialize<DataIDPath>(jsonData, AppInfo.JSON_OPTIONS);

		});

	}

}
