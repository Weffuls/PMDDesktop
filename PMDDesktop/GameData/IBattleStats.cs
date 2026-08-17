namespace PMDDesktop.GameData;

public interface IBattleStats
{

	uint Hp { get; }
	uint PhysicalAttack { get; }
	uint PhysicalDefense { get; }
	uint SpecialAttack { get; }
	uint SpecialDefense { get; }
	uint Speed { get; }

	static BattleStats Add(IBattleStats left, IBattleStats right)
	{

		return new BattleStats()
		{
			Hp = left.Hp + right.Hp,
			PhysicalAttack = left.PhysicalAttack + right.PhysicalAttack,
			PhysicalDefense = left.PhysicalDefense + right.PhysicalDefense,
			SpecialAttack = left.SpecialAttack + right.SpecialAttack,
			SpecialDefense = left.SpecialDefense + right.SpecialDefense,
			Speed = left.Speed + right.Speed
		};

	}

	static BattleStats Subtract(IBattleStats left, IBattleStats right)
	{

		return new BattleStats()
		{
			Hp = left.Hp - right.Hp,
			PhysicalAttack = left.PhysicalAttack - right.PhysicalAttack,
			PhysicalDefense = left.PhysicalDefense - right.PhysicalDefense,
			SpecialAttack = left.SpecialAttack - right.SpecialAttack,
			SpecialDefense = left.SpecialDefense - right.SpecialDefense,
			Speed = left.Speed - right.Speed
		};

	}

}
