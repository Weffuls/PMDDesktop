using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

/// <summary>
/// Holds a reference to an Asset that persists between saves and loads.
/// <para>The existance of an AssetReference implies an expectation that the Asset actually exists at the Location, although it may not, and will throw in that event.</para>
/// </summary>
/// <typeparam name="T">The type of SaveData you're holding a reference to.</typeparam>
/// <remarks>
/// Not caching results here to keep garbage collection intact. Optimization or caching ideas are welcome.
/// </remarks>
[JsonConverter(typeof(AssetReferenceConverterFactory))]
public sealed class AssetReference<T> where T : Asset
{

	/// <summary>
	/// Creates an Asset Reference and sets its location explicitly.
	/// </summary>
	/// <param name="location">The location that this asset is at.</param>
	public AssetReference(T initalValue)
	{

		Location = initalValue.Location;

	}

	/// <summary>
	/// Creates an Asset Reference and sets its location explicitly.
	/// </summary>
	/// <param name="location">The location that this asset is at.</param>
	internal AssetReference(AssetLocation location)
	{

		Location = location;

	}

	[JsonInclude]
	public AssetLocation Location { get; private set; }

	public T GetReference(IAssetIndexable assets)
	{

		return assets.GetAsset<T>(Location);

	}

}
