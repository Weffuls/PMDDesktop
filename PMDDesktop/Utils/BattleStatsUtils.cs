using PMDDesktop.GameData;

namespace PMDDesktop.Utils;

public static class BattleStatsUtils
{

	public static readonly float STANDARD_STAT_MODIFIER_BASE = 2.0f;

	public static BattleStats ToBattleStats(this IBattleStats source)
	{

		return new(source);

	}

	public static ImmutableBattleStats ToImmutableBattleStats(this IBattleStats source)
	{

		return new(source);

	}

	public static uint ApplyStatModifier(uint originStat, int boostLevel)
	{

		float top = STANDARD_STAT_MODIFIER_BASE;
		float bottom = STANDARD_STAT_MODIFIER_BASE;

		if (boostLevel >= 0)
			top += boostLevel;
		else
			bottom -= boostLevel; // Boost Level should be negative here.

		float multiplier = (float)top / bottom;

		uint calculated = (uint)multiplier * originStat;

		return calculated == 0 ? 1 : calculated;

	}

}
