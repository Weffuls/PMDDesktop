using System.Collections.Immutable;
using System.Text;

namespace PMDDesktop;

public class DataID
{

	public static bool CanBeConverted(string inputString)
	{

		if (string.IsNullOrWhiteSpace(inputString))
			return false;

		inputString = inputString.Trim().ToLowerInvariant();

		return inputString.All(ValidChars.Contains);

	}

	private static readonly ImmutableArray<char> ValidChars = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-'];

	public int Length => charData.Length;

	private readonly byte[] charData;

	public DataID(string inputString)
	{

		if (string.IsNullOrWhiteSpace(inputString))
			throw new ArgumentException("You need to provide a non-null or non-whitespace string.", nameof(inputString));

		inputString = inputString.Trim().ToLowerInvariant();

		charData = new byte[inputString.Length];

		for (int charLoop = 0; charLoop < charData.Length; ++charLoop)
		{

			char inputChar = inputString[charLoop];
			int location = ValidChars.IndexOf(inputChar);

			if (location == -1)
				throw new ArgumentException($"{inputChar} cannot be stored in a {nameof(DataID)}.", nameof(inputString));

			charData[charLoop] = (byte)location;

		}

		return;

	}

	public static implicit operator string(DataID data)
	{

		StringBuilder builder = new(data.Length);

		foreach (byte loc in data.charData)
		{
			builder.Append(ValidChars[loc]);
		}

		return builder.ToString();

	}

	public static bool operator ==(DataID left, DataID right)
	{
		return Enumerable.SequenceEqual(left.charData, right.charData);
	}

	public static bool operator !=(DataID left, DataID right)
	{
		return !(left == right);
	}

	public override string ToString()
	{
		return (string)this;
	}

	public override int GetHashCode()
	{

		if (charData.Length < 4)
			return BitConverter.ToInt32(
				[
					charData.ElementAtOrDefault(0),
					charData.ElementAtOrDefault(1),
					charData.ElementAtOrDefault(2),
					charData.ElementAtOrDefault(3)
				]
			);

		return BitConverter.ToInt32(charData);

	}

	public override bool Equals(object? obj)
	{

		if (obj is DataID data)
			return this == data;

		if (obj is string str)
			return (string)this == str;

		return base.Equals(obj);

	}

}
