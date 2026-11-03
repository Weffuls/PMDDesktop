using PMDDesktop;

namespace PMDDesktop.Tests;

public class DataIDTests
{

	[Theory]
	[InlineData("HELLO", "hello")] // Everything should be lowercase.
	[InlineData("  -GReeTings-     ", "-greetings-")] // IDs should be trimmed.
	[InlineData("AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz0123456789-", "aabbccddeeffgghhiijjkkllmmnnooppqqrrssttuuvvwwxxyyzz0123456789-")] // All characters here should work.
	[InlineData("awesome-sauce", "awesome-sauce")] // Nothing should change here.
	[InlineData("W", "w")] // Make sure short strings are okay.
	[InlineData("   O0", "o0")]
	[InlineData("AbC  ", "abc")]
	[InlineData("-67-", "-67-")]
	public void ToAndBackConversions(string input, string expectedOutput)
	{

		Assert.True(DataID.CanBeConverted(input));
		Assert.True(DataID.CanBeConverted(expectedOutput));

		DataID IdFromInput = new(input);

		// It should be equal to the expected output
		Assert.Equal(expectedOutput, IdFromInput);
		Assert.Equal(expectedOutput, IdFromInput.ToString());
		Assert.True(expectedOutput == IdFromInput);
		Assert.False(expectedOutput != IdFromInput);

		// The same ID made from the output should match.
		DataID IdFromOutput = new(expectedOutput);
		Assert.Equal(IdFromOutput, IdFromInput);
		// These are our overridden operator, so we need to try both left/right.
		Assert.True(IdFromOutput == IdFromInput);
		Assert.False(IdFromOutput != IdFromInput);
		Assert.True(IdFromInput == IdFromOutput);
		Assert.False(IdFromInput != IdFromOutput);

		// Test that hash codes match.
		Assert.Equal(IdFromInput.GetHashCode(), IdFromOutput.GetHashCode());
		DataID exampleID = new("this-should-not-be-inline-data");
		Assert.NotEqual(exampleID.GetHashCode(), IdFromInput.GetHashCode());

		// If input!=output, it should not equal input.
		if (input != expectedOutput)
		{
			Assert.NotEqual(input, IdFromInput);
			Assert.NotEqual(input, IdFromInput.ToString());
			Assert.False(input == IdFromInput);
			Assert.False(input == IdFromInput.ToString());
			Assert.True(input != IdFromInput);
			Assert.True(input != IdFromInput.ToString());

			Assert.NotEqual(input, IdFromOutput);
			Assert.NotEqual(input, IdFromOutput.ToString());
			Assert.False(input == IdFromOutput);
			Assert.False(input == IdFromOutput.ToString());
			Assert.True(input != IdFromOutput);
			Assert.True(input != IdFromOutput.ToString());
		}

	}

	[Fact]
	public void GuidsShouldBeOkay()
	{

		for (int loop = 0; loop < 16; ++loop)
		{

			Guid guid = new();

			Assert.True(DataID.CanBeConverted(guid.ToString()));

			DataID _ = new(guid.ToString());

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

		Assert.False(DataID.CanBeConverted(input));
		Assert.Throws<ArgumentException>(() => new DataID(input));

	}

}
