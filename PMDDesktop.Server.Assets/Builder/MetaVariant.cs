using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// Holds a variant and metadata about it.
/// </summary>
/// <param name="variant">The variant we're talking about.</param>
internal class MetaVariant(SpeciesVariant variant)
{

	public SpeciesVariant variant = variant;
	public static implicit operator SpeciesVariant(MetaVariant meta) => meta.variant;

}
