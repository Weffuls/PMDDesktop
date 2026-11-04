using System.Collections.Immutable;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;

namespace PMDDesktop;

[JsonConverter(typeof(DataIDConverter))]
public class DataID
{

	#region Error Messages

	private static string GetInvalidCharacterErrorMessage(char inputChar) =>
		$"{inputChar} cannot be stored in a {nameof(DataID)}.";

	private static string GetBlankStringErrorMessage() =>
		"You need to provide a non-null or non-whitespace string.";

	#endregion

	public static bool CanBeConverted(string inputString)
	{

		return CanBeConverted(inputString, out string? _);

	}

	public static bool CanBeConverted(string inputString, out string? failReason)
	{

		if (string.IsNullOrWhiteSpace(inputString))
		{
			failReason = GetBlankStringErrorMessage();
			return false;
		}

		inputString = inputString.Trim().ToLowerInvariant();

		foreach (char character in inputString)
			if (!ValidChars.Contains(character))
			{
				failReason = GetInvalidCharacterErrorMessage(character);
				return false;
			}

		failReason = null;
		return true;

	}

	private static readonly ImmutableArray<char> ValidChars = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-'];

	public int Length => charData.Length;

	private readonly byte[] charData;

	public DataID(string inputString)
	{

		if (string.IsNullOrWhiteSpace(inputString))
			throw new ArgumentException(GetBlankStringErrorMessage(), nameof(inputString));

		inputString = inputString.Trim().ToLowerInvariant();

		charData = new byte[inputString.Length];

		for (int charLoop = 0; charLoop < charData.Length; ++charLoop)
		{

			char inputChar = inputString[charLoop];
			int location = ValidChars.IndexOf(inputChar);

			if (location == -1)
				throw new ArgumentException(GetInvalidCharacterErrorMessage(inputChar), nameof(inputString));

			charData[charLoop] = (byte)location;

		}

		return;

	}

	#region Operators

	public static implicit operator string(DataID data)
	{

		StringBuilder builder = new(data.Length);

		foreach (byte loc in data.charData)
		{
			builder.Append(ValidChars[loc]);
		}

		return builder.ToString();

	}

	public static bool operator ==(DataID? left, DataID? right)
	{

		// Null checks. If both are null then it's okay.
		if (left is null)
		{
			if (right is null)
				return true;
			return false;
		}
		else if (right is null)
		{
			return false;
		}

		return Enumerable.SequenceEqual(left.charData, right.charData);
	}

	public static bool operator !=(DataID? left, DataID? right)
	{
		return !(left == right);
	}

	#endregion

	#region Standard Object Overrides

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

	#endregion

}
