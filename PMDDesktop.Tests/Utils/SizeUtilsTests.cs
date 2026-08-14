using PMDDesktop.Utils;

namespace PMDDesktop.Tests.Utils;

public class SizeUtilsTests
{

	[Theory]
	[InlineData(0, "0 B")] // It looks nicer to not show a decimal place for bytes.
	[InlineData(1L << 10, "1.00 KiB")] // Powers of 1024, not 1000.
	[InlineData((1L << 20) - 1, "1023.99 KiB")] // Should floor down.
	[InlineData(1L << 20, "1.00 MiB")]
	[InlineData(1L << 30, "1.00 GiB")]
	[InlineData(1L << 40, "1.00 TiB")]
	[InlineData(1L << 50, "1.00 PiB")]
	[InlineData((1L << 60) - 1, "1023.99 PiB")]
	[InlineData(1L << 60, "1.00 EiB")]
	public void ByteSizeToHumanReadableTests(long bytes, string expectedResult)
	{

		string result = SizeUtils.ByteSizeToHumanReadable(bytes);

		Assert.Equal(expectedResult, result);

	}

}
