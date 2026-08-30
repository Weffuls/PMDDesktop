using System.Collections.Immutable;

namespace PMDDesktop.Server.Assets.Data;

public abstract class SpeciesVisual : Asset
{

	/// <summary>
	/// Should this visual be shown for the shiny variant instead?
	/// </summary>
	public bool Shiny { get; internal set; }

	/// <summary>
	/// A list of forms this visual should appear in menus to be selectable for.
	/// </summary>
	public required ImmutableArray<AssetReference<SpeciesForm>> forForms;

	internal SpeciesVisual(AssetLocation location) : base(location)
	{



	}

}
