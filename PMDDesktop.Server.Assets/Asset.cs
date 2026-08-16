using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

public abstract class Asset(AssetLocation location)
{

	[JsonIgnore]
	public AssetLocation Location { get; internal set; } = location;

	/// <summary>
	/// The AssetManager this Asset belongs to. Can be null if not yet assigned to an AssetManager.
	/// </summary>
	/// <remarks>
	/// To avoid entering an invalid state, do not interchange Assets between AssetManagers.
	/// </remarks>
	[JsonIgnore]
	public AssetManager? Manager { get; internal set; }

	public override string ToString()
	{
		return $"[Asset:{GetType().Name}:{Location}]";
	}

}
