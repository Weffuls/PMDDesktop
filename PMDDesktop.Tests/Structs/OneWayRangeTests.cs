using PMDDesktop.Structs;

namespace PMDDesktop.Tests.Structs;

public class OneWayRangeTests
{

	[Theory]
	[InlineData(-532.725f, 0.0f)] // Should clamp up to 0.0f
	[InlineData(591.410f, 1.0f)] // Should clamp down to 1.0f
	[InlineData(0.5f, 0.5f)] // Should preserve value.
	[InlineData(float.Epsilon, float.Epsilon)] // Should preserve value.
	public void ClampingExpectations(float inital, float expectation)
	{

		OneWayRange range = inital;

		Assert.Equal(expectation, range, 5);

	}

	[Theory]

	[InlineData(0, 3, 0.0f)] // 0 out of 3
	[InlineData(1, 3, 1.0f / 3.0f)] // 1 out of 3
	[InlineData(4, 5, 0.8f)] // 4 out of 5.

	[InlineData(-3, 1, 0.0f)] // Negative numbers should be 0.
	[InlineData(10, 5, 1.0f)] // Positive numbers over the target should be 1.

	[InlineData(0, 0, 0.0f)] // 0 of 0 should be 0.
	[InlineData(-3, 0, 0.0f)] // Anything 0 or below of 0 should be 0.
	[InlineData(5, 0, 1.0f)] // Anything above 0 of 0 should be 1.
	public void PercentageTests(float current, float end, float expected)
	{

		OneWayRange range = new(current, end);

		Assert.Equal(expected, range, 5);

	}

}
