using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("portraits")]
public sealed class SpeciesPortraits : SpeciesVisual
{
	internal SpeciesPortraits(AssetLocation location) : base(location) { }

	[JsonConstructor]
	private SpeciesPortraits() : base() { }

}
