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
	/// <returns>A string formatted like "64KiB"</returns>
	public static string ByteSizeToHumanReadable(long byteCount)
	{

		float floatingSize = byteCount;

		int sizeIndex = 0;

		while (floatingSize >= 1024)
		{

			floatingSize /= 1024;
			++sizeIndex;

		}

		return $"{floatingSize:N2} {SIZE_NAMES[sizeIndex]}";

	}

}
