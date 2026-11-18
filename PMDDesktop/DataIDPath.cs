using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop;

/// <summary>
/// <para>A class for holding a path or location using DataIDs.</para>
/// </summary>
/// <param name="pathNodes">Array of DataIDs to use as the path.</param>
[JsonConverter(typeof(DataIDPathConverter))]
public class DataIDPath : IEnumerable<DataID>
{

	/// <summary>
	/// The character (byte) that represents directories in paths. This is a "/".
	/// </summary>
	public static readonly byte SPLIT_CHAR = 0x2F;

	public DataIDPath(params IEnumerable<DataID> pathNodes)
	{

		PathNodes = [.. pathNodes];

		if (PathNodes.Length == 0)
			throw new ArgumentException($"{nameof(DataIDPath)} cannot have 0 {nameof(DataID)}s.", nameof(pathNodes));

	}

	/// <summary>
	/// <para>The <see cref="DataID"/>s that make up this <see cref="DataIDPath"/>.</para>
	/// <para>Presented in order from left-to-right.</para>
	/// </summary>
	public ImmutableArray<DataID> PathNodes { get; private init; }

	/// <summary>
	/// The total number of bytes that make up all <see cref="DataID"/> in this path, plus the "/" between them.
	/// </summary>
	public int CharLength { get => PathNodes.Sum(node => node.Length + 1) - 1; }

	/// <summary>
	/// The count of <see cref="DataID"/>s in this <see cref="DataIDPath"/>; like <see cref="PathNodes.Length"/>.
	/// </summary>
	public int NodeCount { get => PathNodes.Length; }

	public override bool Equals([NotNullWhen(true)] object? obj)
	{

		if (obj is DataIDPath location)
			return location == this;

		return base.Equals(obj);

	}

	/// <summary>
	/// Check that two <see cref="DataIDPath"/>s have the same path or are both null.
	/// </summary>
	/// <param name="left">Left <see cref="DataIDPath"/> or null.</param>
	/// <param name="right">Right <see cref="DataIDPath"/> or null.</param>
	/// <returns>True if both <see cref="DataIDPath"/>s have the same path, or they are both null.</returns>
	/// <remarks>Does not compare by reference, but by the value of each <see cref="DataID"/> in <see cref="PathNodes"/>.</remarks>
	public static bool operator ==(DataIDPath? left, DataIDPath? right)
	{

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

		return Enumerable.SequenceEqual(left.PathNodes, right.PathNodes);

	}

	/// <summary>
	/// Check that two <see cref="DataIDPath"/>s have the different paths or not both null.
	/// </summary>
	/// <param name="left">Left <see cref="DataIDPath"/> or null.</param>
	/// <param name="right">Right <see cref="DataIDPath"/> or null.</param>
	/// <returns>True if the <see cref="DataIDPath"/>s are not both null and have different paths.</returns>
	/// <remarks>Does not compare by reference, but by the value of each <see cref="DataID"/> in <see cref="PathNodes"/>.</remarks>
	public static bool operator !=(DataIDPath? left, DataIDPath? right)
	{
		return !(left == right);
	}

	/// <summary>
	/// Append a <see cref="DataID"/> onto a new <see cref="DataIDPath"/>.
	/// </summary>
	/// <param name="original">The original <see cref="DataIDPath"/> to append to.</param>
	/// <param name="append">The <see cref="DataID"/> to append.</param>
	/// <returns></returns>
	/// <remarks>This only works with the appended data on the right side, because it appends the <see cref="DataID"/>s to the right-side.</remarks>
	public static DataIDPath operator +(DataIDPath original, DataID append)
	{
		return new([.. original.PathNodes, append]);
	}

	/// <summary>
	/// Append multiple <see cref="DataID"/>s onto a new <see cref="DataIDPath"/>.
	/// </summary>
	/// <param name="original">The original <see cref="DataIDPath"/> to append to.</param>
	/// <param name="append">An enumerable of <see cref="DataID"/>s to append.</param>
	/// <returns></returns>
	/// <remarks>This only works with the appended data on the right side, because it appends the <see cref="DataID"/>s to the right-side.</remarks>
	public static DataIDPath operator +(DataIDPath original, IEnumerable<DataID> append)
	{
		return new([.. original.PathNodes, .. append]);
	}

	public override int GetHashCode()
	{
		return PathNodes.Length switch
		{
			1 => HashCode.Combine(PathNodes[0]),
			2 => HashCode.Combine(PathNodes[0], PathNodes[1]),
			3 => HashCode.Combine(PathNodes[0], PathNodes[1], PathNodes[2]),
			4 => HashCode.Combine(PathNodes[0], PathNodes[1], PathNodes[2], PathNodes[3]),
			5 => HashCode.Combine(PathNodes[0], PathNodes[1], PathNodes[2], PathNodes[3], PathNodes[4]),
			6 => HashCode.Combine(PathNodes[0], PathNodes[1], PathNodes[2], PathNodes[3], PathNodes[4], PathNodes[5]),
			7 => HashCode.Combine(PathNodes[0], PathNodes[1], PathNodes[2], PathNodes[3], PathNodes[4], PathNodes[5], PathNodes[6]),
			_ => HashCode.Combine(PathNodes[0], PathNodes[1], PathNodes[2], PathNodes[3], PathNodes[4], PathNodes[5], PathNodes[6], PathNodes[7]),
		};
	}

	/// <summary>
	/// <para>Convert this DataIDPath to a string that's shaped like a path.</para>
	/// <para>For example: "node1/node2"</para>
	/// </summary>
	public override string ToString()
	{
		return (string)this;
	}

	public IEnumerator<DataID> GetEnumerator()
	{
		return ((IEnumerable<DataID>)PathNodes).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)PathNodes).GetEnumerator();
	}

	/// <summary>
	/// <para>Cast a DataIDPath to a string that's shaped like a path.</para>
	/// <para>For example: "node1/node2"</para>
	/// </summary>
	/// <param name="path">The path to cast to string.</param>
	public static explicit operator string(DataIDPath path)
	{

		return string.Join((char)SPLIT_CHAR, path.PathNodes);

	}

	/// <summary>
	/// <para>Convert <see cref="ReadOnlySpan{byte}"/> to a <see cref="DataIDPath"/>.</para>
	/// <para>Will create new <see cref="DataID"/> objects based on splits by <see cref="SPLIT_CHAR"/> ('/').</para>
	/// </summary>
	/// <param name="bytes"></param>
	/// <remarks>Note that this is very strict, and may throw an <see cref="InvalidCastException"/> if any part cannot be turned into a <see cref="DataID"/>.</remarks>
	public static explicit operator DataIDPath(ReadOnlySpan<byte> bytes)
	{

		MemoryExtensions.SpanSplitEnumerator<byte> parts = bytes.Split(SPLIT_CHAR);
		Queue<DataID> datas = [];

		while (parts.MoveNext())
		{
			ReadOnlySpan<byte> partBytes = bytes[parts.Current];

			if (!DataID.CanBeConverted(partBytes, out string? failReason))
				throw new InvalidCastException(failReason);

			datas.Enqueue(new(partBytes));

		}

		return new DataIDPath(datas);

	}

	/// <summary>
	/// Turn a <see cref="DataIDPath"/> into a <see cref="ReadOnlySpan<byte>"/>, consisting of all the <see cref="PathNodes"/> in the object seperated by <see cref="SPLIT_CHAR"/> ('/').
	/// </summary>
	/// <param name="path">The path to convert.</param>
	public static implicit operator ReadOnlySpan<byte>(DataIDPath path)
	{

		Span<byte> bytes = new byte[path.CharLength];

		int currentPosition = 0;

		for (int currentNode = 0; currentNode < path.PathNodes.Length; ++currentNode)
		{

			ReadOnlySpan<byte> nodeBytes = path.PathNodes[currentNode];

			nodeBytes.CopyTo(bytes.Slice(currentPosition, nodeBytes.Length));

			if (currentNode != path.PathNodes.Length - 1)
				bytes[currentPosition + nodeBytes.Length] = SPLIT_CHAR;

			currentPosition += nodeBytes.Length + 1;

		}

		return bytes;

	}

}
