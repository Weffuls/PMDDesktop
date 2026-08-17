namespace PMDDesktop.GameData;

public class BattleStats : IBattleStats
{

	public BattleStats()
	{

	}

	public BattleStats(IBattleStats source)
	{

		Hp = source.Hp;
		PhysicalAttack = source.PhysicalAttack;
		PhysicalDefense = source.PhysicalDefense;
		SpecialAttack = source.SpecialAttack;
		SpecialDefense = source.SpecialDefense;
		Speed = source.Speed;

	}

	public BattleStats(uint hp, uint physicalAttack, uint physicalDefense, uint speed, uint specialAttack, uint specialDefense)
	{

		Hp = hp;
		PhysicalAttack = physicalAttack;
		PhysicalDefense = physicalDefense;
		SpecialAttack = specialAttack;
		SpecialDefense = specialDefense;
		Speed = speed;

	}

	public uint Hp { get; set; }
	public uint PhysicalAttack { get; set; }
	public uint PhysicalDefense { get; set; }
	public uint SpecialAttack { get; set; }
	public uint SpecialDefense { get; set; }
	public uint Speed { get; set; }

	public static BattleStats operator +(BattleStats left, IBattleStats right) => IBattleStats.Add(left, right);
	public static BattleStats operator +(IBattleStats left, BattleStats right) => IBattleStats.Add(left, right);
	public void operator +=(IBattleStats right)
	{
		Hp += right.Hp;
		PhysicalAttack += right.PhysicalAttack;
		PhysicalDefense += right.PhysicalDefense;
		SpecialAttack += right.SpecialAttack;
		SpecialDefense += right.SpecialDefense;
		Speed += right.Speed;
	}
	public static BattleStats operator -(BattleStats left, IBattleStats right) => IBattleStats.Subtract(left, right);
	public static BattleStats operator -(IBattleStats left, BattleStats right) => IBattleStats.Subtract(left, right);
	public void operator -=(IBattleStats right)
	{
		Hp -= right.Hp;
		PhysicalAttack -= right.PhysicalAttack;
		PhysicalDefense -= right.PhysicalDefense;
		SpecialAttack -= right.SpecialAttack;
		SpecialDefense -= right.SpecialDefense;
		Speed -= right.Speed;
	}

}
