using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("sprites")]
public sealed class SpeciesSprites : SpeciesVisual
{
	internal SpeciesSprites(AssetLocation location) : base(location) { }

	[JsonConstructor]
	private SpeciesSprites() : base() { }

}
