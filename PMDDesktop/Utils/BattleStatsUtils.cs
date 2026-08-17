using PMDDesktop.GameData;

namespace PMDDesktop.Utils;

public static class BattleStatsUtils
{

	public static BattleStats ToBattleStats(this IBattleStats source)
	{

		return new(source);

	}

	public static ImmutableBattleStats ToImmutableBattleStats(this IBattleStats source)
	{

		return new(source);

	}

}
