using PMDDesktop;

namespace PMDDesktop.Tests;

public class DataIDPathTests
{

	[Theory]
	[InlineData("node1/node2", new string[] { "node1", "node2" })]
	[InlineData("node1", new string[] { "node1" })]
	[InlineData("hello/to/you", new string[] { "   hello", "  to  ", "you   " })]
	[InlineData("h/e/l/l/o/-/t/o/-/y/o/u", new string[] { "h", "e", "l", "l", "o", "-", "t", "o", "-", "y", "o", "u" })]
	public void StringConversions(string expectedString, string[] nodeSources)
	{

		IEnumerable<DataID> nodes = nodeSources.Select(source => new DataID(source));

		DataIDPath path = new(nodes);

		Assert.Equal(expectedString, path.ToString());

	}

	[Theory]
	[InlineData([new string[] { "node1", "", "node3" }])]
	[InlineData([new string[] { "node1", "node2!", "node3" }])]
	[InlineData([new string[] { "" }])]
	[InlineData([new string[] { "node1/node2", "node3" }])]
	[InlineData([new string[] { "node1", "node 2", "node3" }])]
	[InlineData([new string[] { }])]
	public void InvalidPaths(string[] nodeSources)
	{

		Assert.ThrowsAny<Exception>(() =>
		{

			IEnumerable<DataID> nodes = nodeSources.Select(source => new DataID(source));

			DataIDPath path = new(nodes);

		});

	}

	[Theory]
	[InlineData(true, new string[] { "    node1", "node2" }, new string[] { "node1    ", "node2" })]
	[InlineData(false, new string[] { "node1", "node2" }, new string[] { "node1", "node3" })]
	[InlineData(true, new string[] { "NODEONE", "node2" }, new string[] { "nodeone", "node2" })]
	[InlineData(false, new string[] { "NODEONE" }, new string[] { "NODE", "ONE" })]
	public void EqualityTests(bool expected, string[] sources1, string[] sources2)
	{

		IEnumerable<DataID> nodes1 = sources1.Select(source => new DataID(source));
		DataIDPath path1 = new(nodes1);

		IEnumerable<DataID> nodes2 = sources2.Select(source => new DataID(source));
		DataIDPath path2 = new(nodes2);

		if (expected)
		{
			Assert.True(path1 == path2);
			Assert.True(path2 == path1);
			Assert.False(path1 != path2);
			Assert.False(path2 != path1);
			Assert.Equal(path1, path2);
			Assert.Equal(path2, path1);
			Assert.True(path1.Equals(path2));
			Assert.True(path2.Equals(path1));
		}
		else
		{
			Assert.False(path1 == path2);
			Assert.False(path2 == path1);
			Assert.True(path1 != path2);
			Assert.True(path2 != path1);
			Assert.NotEqual(path1, path2);
			Assert.NotEqual(path2, path1);
			Assert.False(path1.Equals(path2));
			Assert.False(path2.Equals(path1));
		}

	}

	[Theory]
	[InlineData(true, new string[] { "hi" }, new string[] { "hi" })]
	[InlineData(false, new string[] { "hi" }, null)]
	[InlineData(false, null, new string[] { "hi" })]
	[InlineData(true, null, null)]
	public void NullTests(bool expected, string[]? sources1, string[]? sources2)
	{

		IEnumerable<DataID>? nodes1 = sources1?.Select(source => new DataID(source));
		DataIDPath? path1 = nodes1 is not null ? new(nodes1) : null;

		IEnumerable<DataID>? nodes2 = sources2?.Select(source => new DataID(source));
		DataIDPath? path2 = nodes2 is not null ? new(nodes2) : null;

		if (expected)
		{
			Assert.True(path1 == path2);
			Assert.True(path2 == path1);
			Assert.False(path1 != path2);
			Assert.False(path2 != path1);
			Assert.Equal(path1, path2);
			Assert.Equal(path2, path1);
		}
		else
		{
			Assert.False(path1 == path2);
			Assert.False(path2 == path1);
			Assert.True(path1 != path2);
			Assert.True(path2 != path1);
			Assert.NotEqual(path1, path2);
			Assert.NotEqual(path2, path1);
		}

	}

}
