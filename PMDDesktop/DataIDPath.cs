using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop;

/// <summary>
/// <para>A class for holding a path or location using DataIDs.</para>
/// </summary>
/// <param name="pathNodes">Array of DataIDs to use as the path.</param>
[JsonConverter(typeof(DataIDPathConverter))]
public class DataIDPath
{
	public static readonly char SPLIT_CHAR = '/';

	public DataIDPath(params IEnumerable<DataID> pathNodes)
	{

		PathNodes = [.. pathNodes];

		if (PathNodes.Length == 0)
			throw new ArgumentException($"{nameof(DataIDPath)} cannot have 0 {nameof(DataID)}s.", nameof(pathNodes));

	}

	public ImmutableArray<DataID> PathNodes { get; private init; }

	public DataID Tail { get => PathNodes.Last(); }

	public override bool Equals([NotNullWhen(true)] object? obj)
	{

		if (obj is DataIDPath location)
			return location == this;

		return base.Equals(obj);

	}

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

	public static bool operator !=(DataIDPath? left, DataIDPath? right)
	{
		return !(left == right);
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
		return string.Join(SPLIT_CHAR, PathNodes);
	}

	/// <summary>
	/// <para>Cast a DataIDPath to a string that's shaped like a path.</para>
	/// <para>For example: "node1/node2"</para>
	/// </summary>
	/// <param name="path">The path to cast to string.</param>
	public static implicit operator string(DataIDPath path)
	{

		return path.ToString();

	}

}
