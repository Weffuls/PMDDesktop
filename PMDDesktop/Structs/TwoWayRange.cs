namespace PMDDesktop.Structs;

public struct TwoWayRange
{

	public float Value { get; private set => field = Clamp(value); }

	private static float Clamp(float input)
	{

		return Math.Clamp(input, -1.0f, 1.0f);

	}

}
