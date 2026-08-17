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

}
