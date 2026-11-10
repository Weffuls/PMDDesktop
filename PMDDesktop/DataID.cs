using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace PMDDesktop;

[JsonConverter(typeof(DataIDConverter))]
public class DataID
{

	#region Error Messages

	private static string GetInvalidCharacterErrorMessage(byte inputByte) =>
		$"{(char)inputByte} ({inputByte}) cannot be stored in a {nameof(DataID)}.";

	private static string GetBlankInputSpanErrorMessage() =>
		"You need to provide a non-null input.";

	#endregion

	public static bool CanBeConverted(ReadOnlySpan<byte> inputSpan)
	{

		return CanBeConverted(inputSpan, out string? _);

	}

	public static bool CanBeConverted(ReadOnlySpan<byte> inputSpan, out string? failReason)
	{

		if (inputSpan.Length == 0)
		{
			failReason = GetBlankInputSpanErrorMessage();
			return false;
		}

		foreach (byte char8 in inputSpan)
			if (!IsValidByte(char8))
			{
				failReason = GetInvalidCharacterErrorMessage(char8);
				return false;
			}

		failReason = null;
		return true;

	}

	public static bool IsValidByte(byte char8)
	{

		// this is: a-z
		// first because it's most common imo
		if (0x61 <= char8 && char8 <= 0x7A)
			return true;

		// this is: 0-9
		if (0x30 <= char8 && char8 <= 0x39)
			return true;

		// this is: - (hypen)
		if (0x2D == char8)
			return true;

		return false;

	}

	public int Length => charData.Length;

	private readonly byte[] charData;

	public DataID(ReadOnlySpan<byte> inputSpan)
	{

		if (inputSpan.Length == 0)
			throw new ArgumentException(GetBlankInputSpanErrorMessage(), nameof(inputSpan));

		foreach (byte char8 in inputSpan)
			if (!IsValidByte(char8))
				throw new ArgumentException(GetInvalidCharacterErrorMessage(char8), nameof(inputSpan));

		charData = inputSpan.ToArray();

		return;

	}

	#region Operators

	[return: NotNullIfNotNull(nameof(data))]
	public static explicit operator string?(DataID? data)
	{

		if (data is null)
			return null;

		return new(Encoding.UTF8.GetChars(data.charData));

	}

	[return: NotNullIfNotNull(nameof(str))]
	public static explicit operator DataID?(string? str)
	{

		if (str is null)
			return null;

		return new(Encoding.UTF8.GetBytes(str));

	}

	public static explicit operator DataID(ReadOnlySpan<byte> bytes)
	{
		return new(bytes);
	}

	public static implicit operator ReadOnlySpan<byte>(DataID data)
	{

		return new(data.charData);

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

		if (obj is byte[] bytes)
			return Enumerable.SequenceEqual(charData, bytes);

		if (obj is string str)
			return (string)this == str;

		return base.Equals(obj);

	}

	#endregion

}
