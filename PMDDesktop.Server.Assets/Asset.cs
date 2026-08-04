using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets;

public abstract class Asset(AssetLocation location)
{

	[JsonIgnore]
	public AssetLocation Location { get; internal set; } = location;

	public override string ToString()
	{
		return $"[Asset:{GetType().Name}:{Location}]";
	}

}
