using System.Text.Json;
using PMDDesktop;

namespace PMDDesktop.Tests;

public class DataIDConverterTests
{

	[Theory]
	[InlineData("hello")]
	[InlineData("-greetings-")]
	[InlineData("abcdefghijklmnopqrstuvwxyz0123456789-")]
	[InlineData("awesome-sauce")]
	[InlineData("w")]
	[InlineData("o0")]
	[InlineData("abc")]
	[InlineData("-67-")]
	public void ToAndBackJsonConversions(string input)
	{

		DataID original = new((DataID)input);

		string jsonData = JsonSerializer.Serialize(original, AppInfo.JSON_OPTIONS);

		DataID? deserialized = JsonSerializer.Deserialize<DataID>(jsonData, AppInfo.JSON_OPTIONS);

		Assert.NotNull(deserialized);

		Assert.Equal(original, deserialized);
		Assert.True(original == deserialized);
		Assert.Equal(original.GetHashCode(), deserialized.GetHashCode());

	}

	[Theory]
	[InlineData("hello", true)]
	[InlineData("abcdefghijklmnopqrstuvwxyz0123456789-", true)]
	[InlineData("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789-", false)]
	[InlineData("   O0", false)]
	[InlineData("unawesome sauce", false)]
	[InlineData("(cool)", false)]
	[InlineData("#O0", false)]
	[InlineData("awesome-sauce", true)]
	public void JsonExceptionTests(string input, bool isValid)
	{

		string data = JsonSerializer.Serialize(input);

		if (!isValid)
		{

			Assert.Throws<JsonException>(() =>
			{

				DataID? id = JsonSerializer.Deserialize<DataID>(data, AppInfo.JSON_OPTIONS);

			});

		}
		else
		{

			DataID? id = JsonSerializer.Deserialize<DataID>(data, AppInfo.JSON_OPTIONS);
			Assert.NotNull(id);

		}

	}

}
