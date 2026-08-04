namespace PMDDesktop.Structs;

public struct OneWayRange
{

	public OneWayRange(float inital)
	{
		Value = inital;
	}

	public OneWayRange(float current, float end)
	{
		if (end <= 0.0f)
			current = 0.0f;
		Value = current / end;
	}

	public float Value { get; private set => field = Clamp(value); }

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
