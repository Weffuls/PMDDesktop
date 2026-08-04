using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("species")]
public class Species : Asset
{

	[JsonConstructor]
	private Species() : this(new()) { }

	internal Species(AssetLocation location) : base(location) { }

}
