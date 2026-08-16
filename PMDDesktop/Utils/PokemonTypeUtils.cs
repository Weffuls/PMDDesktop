using PMDDesktop.GameData;

namespace PMDDesktop.Utils;

public static class PokemonTypeUtils
{

	public static readonly float INITIAL_MULTIPLIER = 1.0f;
	public static readonly float WEAKNESS_MULTIPLIER = 2.0f;
	public static readonly float RESISTANCE_MULTIPLIER = 0.5f;
	public static readonly float IMMUNITY_MULTIPLIER = 0.0f;

	public static bool IsWeakTo(this IPokemonType defender, IPokemonType attacker)
	{

		return defender.Weaknesses.Contains(attacker);

	}

	public static bool IsResistantTo(this IPokemonType defender, IPokemonType attacker)
	{

		return defender.Resistances.Contains(attacker);

	}

	public static bool IsImmuneTo(this IPokemonType defender, IPokemonType attacker)
	{

		return defender.Immunities.Contains(attacker);

	}

	public static float GetDamageMultiplier(IEnumerable<IPokemonType> defendingTypes, IEnumerable<IPokemonType> attackingTypes)
	{

		float total = INITIAL_MULTIPLIER;

		foreach (IPokemonType attacker in attackingTypes)
			total *= GetDamageMultiplier(defendingTypes, attacker);

		return total;

	}

	public static float GetDamageMultiplier(IEnumerable<IPokemonType> defendingTypes, IPokemonType attackingType)
	{

		float total = INITIAL_MULTIPLIER;

		foreach (IPokemonType defender in defendingTypes)
			total *= GetDamageMultiplier(defender, attackingType);

		return total;

	}

	public static float GetDamageMultiplier(IPokemonType defendingType, IEnumerable<IPokemonType> attackingTypes)
	{

		float total = INITIAL_MULTIPLIER;

		foreach (IPokemonType attacker in attackingTypes)
			total *= GetDamageMultiplier(defendingType, attacker);

		return total;

	}

	public static float GetDamageMultiplier(IPokemonType defendingType, IPokemonType attackingType)
	{

		if (defendingType.IsWeakTo(attackingType))
			return WEAKNESS_MULTIPLIER;
		else if (defendingType.IsResistantTo(attackingType))
			return RESISTANCE_MULTIPLIER;
		else if (defendingType.IsImmuneTo(attackingType))
			return IMMUNITY_MULTIPLIER;
		else
			return INITIAL_MULTIPLIER;

	}

}
