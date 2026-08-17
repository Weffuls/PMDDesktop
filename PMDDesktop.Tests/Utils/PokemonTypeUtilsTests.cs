using PMDDesktop.Utils;

namespace PMDDesktop.Tests.Utils;

public class PokemonTypeUtilsTests
{

	[Fact]
	public void IsWeakToTrue()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new()
		{
			Weaknesses = [attacker]
		};

		Assert.True(defender.IsWeakTo(attacker));

	}

	[Fact]
	public void IsWeakToFalse()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new();

		Assert.False(defender.IsWeakTo(attacker));

	}

	[Fact]
	public void IsResistantToTrue()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new()
		{
			Resistances = [attacker]
		};

		Assert.True(defender.IsResistantTo(attacker));

	}

	[Fact]
	public void IsResistantToFalse()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new();

		Assert.False(defender.IsResistantTo(attacker));

	}

	[Fact]
	public void IsImmuneToTrue()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new()
		{
			Immunities = [attacker]
		};

		Assert.True(defender.IsImmuneTo(attacker));

	}

	[Fact]
	public void IsImmuneToFalse()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new();

		Assert.False(defender.IsImmuneTo(attacker));

	}

	[Fact]
	public void DamageMultiplierStandard()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new();

		float expected = 1.0f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, [attacker]));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], [attacker]));

	}

	[Fact]
	public void DamageMultiplierHalf()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new()
		{
			Resistances = [attacker]
		};

		float expected = 0.5f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, [attacker]));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], [attacker]));

	}

	[Fact]
	public void DamageMultiplierDouble()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new()
		{
			Weaknesses = [attacker]
		};

		float expected = 2.0f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, [attacker]));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], [attacker]));

	}

	[Fact]
	public void DamageMultiplierNull()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender = new()
		{
			Immunities = [attacker]
		};

		float expected = 0.0f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier(defender, [attacker]));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender], [attacker]));

	}

	[Fact]
	public void DamageMultiplierCancelOutStandard()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender1 = new()
		{
			Weaknesses = [attacker],
		};
		ExamplePokemonType defender2 = new()
		{
			Resistances = [attacker]
		};

		float expected = 1.0f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender1, defender2], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender1, defender2], [attacker]));

	}

	[Fact]
	public void DamageMultiplierQuadruple()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender1 = new()
		{
			Weaknesses = [attacker],
		};
		ExamplePokemonType defender2 = new()
		{
			Weaknesses = [attacker]
		};

		float expected = 4.0f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender1, defender2], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender1, defender2], [attacker]));

	}

	[Fact]
	public void DamageMultiplierQuarter()
	{

		ExamplePokemonType attacker = new();
		ExamplePokemonType defender1 = new()
		{
			Resistances = [attacker],
		};
		ExamplePokemonType defender2 = new()
		{
			Resistances = [attacker]
		};

		float expected = 0.25f;

		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender1, defender2], attacker));
		Assert.Equal(expected, PokemonTypeUtils.GetDamageMultiplier([defender1, defender2], [attacker]));

	}

}
