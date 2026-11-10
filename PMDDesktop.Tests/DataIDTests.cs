using System.Text;
using PMDDesktop;

namespace PMDDesktop.Tests;

public class DataIDTests
{

	[Theory]
	[InlineData("hello")] // Everything should be lowercase.
	[InlineData("-greetings-")] // IDs should be trimmed.
	[InlineData("abcdefghijklmnopqrstuvwxyz0123456789-")] // All characters here should work.
	[InlineData("awesome-sauce")] // Nothing should change here.
	[InlineData("w")] // Make sure short strings are okay.
	[InlineData("o0")]
	[InlineData("abc")]
	[InlineData("-67-")]
	public void ToAndBackConversions(string input)
	{

		Assert.True(DataID.CanBeConverted(Encoding.UTF8.GetBytes(input)));

		DataID IdFromInput = new(Encoding.UTF8.GetBytes(input));

		DataID exampleID = new("this-should-not-be-inline-data"u8);
		Assert.NotEqual(exampleID.GetHashCode(), IdFromInput.GetHashCode());

		Assert.True(input == IdFromInput.ToString());
		Assert.False(input != IdFromInput.ToString());

	}

	[Fact]
	public void GuidsShouldBeOkay()
	{

		for (int loop = 0; loop < 16; ++loop)
		{

			Guid guid = new();

			ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(guid.ToString());

			Assert.True(DataID.CanBeConverted(bytes));

			DataID _ = new(bytes);

		}

	}

	[Theory]
	[InlineData("")]
	[InlineData("          ")]
	[InlineData("\n")]
	[InlineData("use_hyphens_instead_of_underscores")]
	[InlineData("no spaces please")]
	[InlineData("DataID/is/not/a/path")]
	public void UnconvertableShouldThrow(string input)
	{

		Assert.False(DataID.CanBeConverted(Encoding.UTF8.GetBytes(input)));
		Assert.Throws<ArgumentException>(() => (DataID)input);

	}

}
