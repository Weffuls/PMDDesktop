namespace PMDDesktop.Server.Assets.Data;

/// <summary>
/// <para>Class intended to be nested in a <see cref="Species"/> asset to detail how they evolve.</para>
/// <para>All evolutions are backwards facing; we start at the top and work our way back to the lowest-level <see cref="Species"/>.</para>
/// </summary>
public sealed class SpeciesEvolutionDetails
{

	internal SpeciesEvolutionDetails()
	{

	}

	/// <summary>
	/// <para>What <see cref="Species"/> does this <see cref="Species"/> evolve from?</para>
	/// </summary>
	public required AssetReference<Species> FromSpecies { get; init; }

	/// <summary>
	/// <para>The minimum level this <see cref="Species"/> evolves at.</para>
	/// <para>If 0, then the <see cref="Species"/> does not require a minimum level.</para>
	/// </summary>
	public int MinimumLevel { get; init; } = 0;

}
