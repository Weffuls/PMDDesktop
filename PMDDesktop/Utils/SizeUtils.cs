namespace PMDDesktop.Utils;

public static class SizeUtils
{

	/// <summary>
	/// Names of data sizes, assuming an increase of 1024 each time.
	/// </summary>
	private static readonly string[] SIZE_NAMES = ["B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB"];
	// Hopefully this never goes above a few GiB, but we should support all of the long numbers anyways.

	/// <summary>
	/// Returns a human readable size string for the input amount of bytes.
	/// </summary>
	/// <param name="byteCount">The count of bytes.</param>
	/// <returns>A string formatted like "64 KiB"</returns>
	public static string ByteSizeToHumanReadable(long byteCount)
	{

		// There may be a more elegant way to do this.
		// Decimal is used because EiB needs it for precision.
		decimal currentDecimal = byteCount;

		int sizeIndex = 0;

		while (currentDecimal >= 1024.0m)
		{

			currentDecimal /= 1024.0m;
			++sizeIndex;

		}

		if (sizeIndex == 0)
			return $"{byteCount} {SIZE_NAMES[sizeIndex]}";

		currentDecimal = Math.Floor(currentDecimal * 100.0m) / 100.0m;

		return $"{currentDecimal:0.00} {SIZE_NAMES[sizeIndex]}";

	}

}
