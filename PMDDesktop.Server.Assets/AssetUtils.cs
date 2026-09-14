using System.Text.Json;

namespace PMDDesktop.Server.Assets;

public static class AssetUtils
{

	/// <summary>
	/// Extremely slow function that checks if two serialized assets have the same output. Compares by directly comparing Serialized JSON bytes.
	/// </summary>
	/// <param name="asset1">Asset 1</param>
	/// <param name="asset2">Asset 2</param>
	/// <returns>Were both assets serialized the same?</returns>
	public static bool HoldsIdenticalData(Asset asset1, Asset asset2)
	{

		byte[] asset1Bytes = JsonSerializer.SerializeToUtf8Bytes(asset1, asset1.GetType(), AppInfo.JSON_OPTIONS);
		byte[] asset2Bytes = JsonSerializer.SerializeToUtf8Bytes(asset2, asset2.GetType(), AppInfo.JSON_OPTIONS);


		if (asset1Bytes.Length != asset2Bytes.Length)
			return false;

		for (int index = 0; index < asset1Bytes.Length; ++index)
			if (asset1Bytes[index] != asset2Bytes[index])
				return false;

		return true;

	}

}
