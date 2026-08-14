namespace PMDDesktop.Structs;

public struct OneWayRange
{

	public OneWayRange(float inital)
	{
		Value = inital;
	}

	public OneWayRange(float current, float end)
	{
		Value = CalculatePercentage(current, end);
	}

	public float Value { get; private set => field = Clamp(value); }

	/// <summary>
	/// Claculates
	/// </summary>
	/// <returns></returns>
	private static float CalculatePercentage(float current, float end)
	{

		if (end <= 0.0f)
		{
			if (current <= 0.0f)
				return 0.0f;
			else
				return 1.0f;
		}

		return Clamp(current / end);

	}

	private static float Clamp(float input)
	{

		return Math.Clamp(input, 0.0f, 1.0f);

	}

	public static implicit operator OneWayRange(float input)
	{
		return new OneWayRange(input);
	}

	public static implicit operator float(OneWayRange range)
	{
		return range.Value;
	}

}
