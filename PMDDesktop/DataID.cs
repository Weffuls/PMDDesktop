using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;

namespace PMDDesktop;

/// <summary>
/// <para>Class for holding super normalized "string-like" IDs.</para>
/// <para>All inputs are normalized and will throw if they do not conform to the standard.</para>
/// <para>Additionally, all <see cref="DataID"/>s contain only characters that are safe to use in cross-platform file systems.</para>
/// </summary>
[JsonConverter(typeof(DataIDConverter))]
public class DataID : IEnumerable<byte>
{

	#region Error Messages

	/// <summary>
	/// Reusable error message for invalid characters.
	/// </summary>
	/// <param name="inputByte">Byte that cannot be stored in a <see cref="DataID"/></param>
	/// <returns>Returns an error message stating that <paramref name="inputByte"/> cannot be stored in a DataID.</returns>
	private static string GetInvalidCharacterErrorMessage(byte inputByte) =>
		$"{(char)inputByte} ({inputByte}) cannot be stored in a {nameof(DataID)}.";

	/// <summary>
	/// Reusable error message for empty inputs.
	/// </summary>
	/// <returns>Returns an error message stating that inputs cannot be empty.</returns>
	private static string GetBlankInputSpanErrorMessage() =>
		"You need to provide a non-empty input.";

	#endregion Error Messages

	#region Validity Checks

	/// <summary>
	/// Checks if a <see cref="ReadOnlySpan<byte>"/> could be a valid <see cref="DataID"/>.
	/// </summary>
	/// <param name="inputSpan">The <see cref="ReadOnlySpan<byte>"/> that's being checked and would be converted into a DataID.</param>
	/// <returns>True if it can be converted into a DataID.</returns>
	public static bool CanBeConverted(ReadOnlySpan<byte> inputSpan)
	{

		return CanBeConverted(inputSpan, out string? _);

	}

	/// <summary>
	/// Checks if a <see cref="ReadOnlySpan<byte>"/> could be a valid <see cref="DataID"/>, or outputs a message stating why it can't.
	/// </summary>
	/// <param name="inputSpan">The <see cref="ReadOnlySpan<byte>"/> that's being checked and would be converted into a DataID.</param>
	/// <param name="failReason">When returning false, a message stating why the </param>
	/// <returns>True if it can be converted into a DataID.</returns>
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

	/// <summary>
	/// Determines if a byte can be used as a character in a <see cref="DataID"/>.
	/// </summary>
	/// <param name="char8">A byte representing a UTF-8 char.</param>
	/// <returns>True if it can be used.</returns>
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

	#endregion Validity Checks

	/// <summary>
	/// Count of chars in the <see cref="DataID"/>.
	/// </summary>
	public int Length => charData.Length;

	/// <summary>
	/// The bytes that make up the <see cref="DataID"/>.
	/// </summary>
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

	/// <summary>
	/// Create a string object using a DataID. Will be null if DataID input is null.
	/// </summary>
	/// <param name="data">Input data.</param>
	[return: NotNullIfNotNull(nameof(data))]
	public static explicit operator string?(DataID? data)
	{

		if (data is null)
			return null;

		return new(Encoding.UTF8.GetChars(data.charData));

	}

	/// <summary>
	/// Create a DataID object using a string. Will be null if string input is null.
	/// </summary>
	/// <param name="str">Input data.</param>
	[return: NotNullIfNotNull(nameof(str))]
	public static explicit operator DataID?(string? str)
	{

		if (str is null)
			return null;

		return new(Encoding.UTF8.GetBytes(str));

	}

	/// <summary>
	/// Create a <see cref="DataID"/> using a <see cref="ReadOnlySpan{byte}"/>. This essentially just calls the constructor.
	/// </summary>
	/// <param name="bytes">Input bytes to be stored in the DataID.</param>
	public static explicit operator DataID(ReadOnlySpan<byte> bytes)
	{
		return new(bytes);
	}

	/// <summary>
	/// Get the charData from this <see cref="DataID"/> as a <see cref="ReadOnlySpan{byte}"/>.
	/// </summary>
	/// <param name="data">Input DataID.</param>
	public static implicit operator ReadOnlySpan<byte>(DataID data)
	{

		return new(data.charData);

	}

	/// <summary>
	/// Test that two <see cref="DataID"/>s have the same <see cref="charData"/>, or that they're both null.
	/// </summary>
	/// <param name="left">Left <see cref="DataID"/> or null.</param>
	/// <param name="right">Right <see cref="DataID"/> or null.</param>
	/// <returns>True if they are both null or have the same <see cref="charData"/>.</returns>
	/// <remarks>Both DataIDs are treated equallty in this operator.
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

	/// <summary>
	/// Test that two <see cref="DataID"/>s have different <see cref="charData"/>, or that they're not both null.
	/// </summary>
	/// <param name="left">Left <see cref="DataID"/> or null.</param>
	/// <param name="right">Right <see cref="DataID"/> or null.</param>
	/// <returns>True if they're not both null and if they have different charData.</returns>
	/// <remarks>Both DataIDs are treated equallty in this operator.
	public static bool operator !=(DataID? left, DataID? right)
	{
		return !(left == right);
	}

	#endregion Operators

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

		if (obj is IEnumerable<byte> bytes)
			return Enumerable.SequenceEqual(charData, bytes);

		if (obj is string str)
			return (string)this == str;

		return base.Equals(obj);

	}

	public IEnumerator<byte> GetEnumerator()
	{
		return ((IEnumerable<byte>)charData).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return charData.GetEnumerator();
	}

	#endregion Standard Object Overrides

}
