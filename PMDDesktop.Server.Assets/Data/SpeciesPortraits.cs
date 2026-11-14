using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("portraits")]
public sealed class SpeciesPortraits : SpeciesVisual
{

	[JsonConstructor]
	internal SpeciesPortraits(DataIDPath path) : base(path) { }

}
