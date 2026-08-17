namespace PMDDesktop.GameData;

public interface IBattleStats
{

	uint Hp { get; }
	uint PhysicalAttack { get; }
	uint PhysicalDefense { get; }
	uint SpecialAttack { get; }
	uint SpecialDefense { get; }
	uint Speed { get; }

}
