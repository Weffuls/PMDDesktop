using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Assets.Data;

[AssetFileName("species")]
public sealed class Species : Asset
{

	[JsonConstructor]
	private Species() : this(new()) { }

	internal Species(AssetLocation location) : base(location) { }

	public SpeciesEvolutionDetails? EvolvesFrom { get; internal set; }

}
