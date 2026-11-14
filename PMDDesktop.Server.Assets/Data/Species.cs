using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("species")]
public sealed class Species : Asset
{

	[JsonConstructor]
	internal Species(DataIDPath path) : base(path) { }

	public SpeciesEvolutionDetails? EvolvesFrom { get; internal set; }

}
