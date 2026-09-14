namespace PMDDesktop.GameData;

public struct BattleStats(int hp, int physicalAttack, int physicalDefense, int specialAttack, int specialDefense, int speed)
{

	public static readonly BattleStats Zero = new();
	public static readonly BattleStats One = new(1, 1, 1, 1, 1, 1);
	public static readonly BattleStats MinValue = new(int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue, int.MinValue);
	public static readonly BattleStats MaxValue = new(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);

	public int Hp { get; set; } = hp;
	public int PhysicalAttack { get; set; } = physicalAttack;
	public int PhysicalDefense { get; set; } = physicalDefense;
	public int SpecialAttack { get; set; } = specialAttack;
	public int SpecialDefense { get; set; } = specialDefense;
	public int Speed { get; set; } = speed;

	public static BattleStats Clamp(BattleStats input, BattleStats min, BattleStats max) => new()
	{
		Hp = Math.Clamp(input.Hp, min.Hp, max.Hp),
		PhysicalAttack = Math.Clamp(input.PhysicalAttack, min.PhysicalAttack, max.PhysicalAttack),
		PhysicalDefense = Math.Clamp(input.PhysicalDefense, min.PhysicalDefense, max.PhysicalDefense),
		SpecialAttack = Math.Clamp(input.SpecialAttack, min.SpecialAttack, max.SpecialAttack),
		SpecialDefense = Math.Clamp(input.SpecialDefense, min.SpecialDefense, max.SpecialDefense),
		Speed = Math.Clamp(input.Speed, min.Speed, max.Speed)
	};

	public static BattleStats operator +(BattleStats left, BattleStats right) => new()
	{
		Hp = left.Hp + right.Hp,
		PhysicalAttack = left.PhysicalAttack + right.PhysicalAttack,
		PhysicalDefense = left.PhysicalDefense + right.PhysicalDefense,
		SpecialAttack = left.SpecialAttack + right.SpecialAttack,
		SpecialDefense = left.SpecialDefense + right.SpecialDefense,
		Speed = left.Speed + right.Speed,
	};
	public static BattleStats operator -(BattleStats left, BattleStats right) => new()
	{
		Hp = left.Hp - right.Hp,
		PhysicalAttack = left.PhysicalAttack - right.PhysicalAttack,
		PhysicalDefense = left.PhysicalDefense - right.PhysicalDefense,
		SpecialAttack = left.SpecialAttack - right.SpecialAttack,
		SpecialDefense = left.SpecialDefense - right.SpecialDefense,
		Speed = left.Speed - right.Speed,
	};
	public static BattleStats operator -(BattleStats input) => new()
	{
		Hp = -input.Hp,
		PhysicalAttack = -input.PhysicalAttack,
		PhysicalDefense = -input.PhysicalDefense,
		SpecialAttack = -input.SpecialAttack,
		SpecialDefense = -input.SpecialDefense,
		Speed = -input.Speed,
	};

	public readonly int GetBaseStatTotal()
	{
		return
			Hp +
			PhysicalAttack +
			PhysicalDefense +
			SpecialAttack +
			SpecialDefense +
			Speed;
	}

}
