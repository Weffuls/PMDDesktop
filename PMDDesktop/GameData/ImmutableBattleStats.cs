namespace PMDDesktop.GameData;

public class ImmutableBattleStats : IBattleStats
{

	public ImmutableBattleStats(IBattleStats source)
	{

		Hp = source.Hp;
		PhysicalAttack = source.PhysicalAttack;
		PhysicalDefense = source.PhysicalDefense;
		SpecialAttack = source.SpecialAttack;
		SpecialDefense = source.SpecialDefense;
		Speed = source.Speed;

	}

	public ImmutableBattleStats(uint hp, uint physicalAttack, uint physicalDefense, uint speed, uint specialAttack, uint specialDefense)
	{

		Hp = hp;
		PhysicalAttack = physicalAttack;
		PhysicalDefense = physicalDefense;
		SpecialAttack = specialAttack;
		SpecialDefense = specialDefense;
		Speed = speed;

	}

	public uint Hp { get; init; }
	public uint PhysicalAttack { get; init; }
	public uint PhysicalDefense { get; init; }
	public uint SpecialAttack { get; init; }
	public uint SpecialDefense { get; init; }
	public uint Speed { get; init; }

}
