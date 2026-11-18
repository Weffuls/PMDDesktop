using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("sprites")]
public sealed class SpeciesSprites : SpeciesVisual
{

	[JsonConstructor]
	internal SpeciesSprites(DataIDPath path) : base(path) { }

}
