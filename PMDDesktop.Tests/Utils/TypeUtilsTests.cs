using PMDDesktop.Requests;
using PMDDesktop.Structs;
using PMDDesktop.Utils;
using System.Reflection;

namespace PMDDesktop.Tests.Utils;

public class TypeUtilsTests
{

	#region Testing for Members with Properties

	[Example]
	public float exampleFieldTest;
	[Example]
	public int ExamplePropertyTest { get; set; }
	public double noAttributeFieldTest;
	public long NoAttributePropertyTest { get; set; }

	[Theory]
	[InlineData("exampleFieldTest", true)]
	[InlineData("ExamplePropertyTest", true)]
	[InlineData("noAttributeFieldTest", false)]
	[InlineData("NoAttributePropertyTest", false)]
	public void GetMembersWithAttributeTests(string memberName, bool included)
	{

		IEnumerable<MemberInfo> members =
			typeof(TypeUtilsTests).GetMembersWithAttribute<ExampleAttribute>();

		if (included)
			Assert.Contains(members, (member) => member.Name == memberName);
		else
			Assert.DoesNotContain(members, (member) => member.Name == memberName);

	}

	[Fact]
	public void GetMembersWithAttributeShouldHaveAttribute()
	{

		IEnumerable<MemberInfo> members =
			typeof(TypeUtilsTests).GetMembersWithAttribute<ExampleAttribute>();

		Assert.DoesNotContain(members, (member) => member.GetCustomAttribute<ExampleAttribute>() == null);

	}

	#endregion

	#region Testing for Instanceable Classes

	[Theory]
	// ServerRequest<> is an abstract class, so it shouldn't be included.
	[InlineData(typeof(ServerRequest<>), false)]
	// OneWayRange is a struct, not a class, so it shouldn't be included.
	[InlineData(typeof(OneWayRange), false)]
	// ListSpeciesRequest is a class, so it should be included.
	[InlineData(typeof(ListSpeciesRequest), true)]
	// AppInfo is a static class, so it should not be included.
	[InlineData(typeof(AppInfo), false)]
	public static void GetInstanceableClassesTests(Type containsType, bool shouldContain)
	{

		IEnumerable<Type> results =
			TypeUtils.GetInstanceableClasses();

		if (shouldContain)
			Assert.Contains(containsType, results);
		else
			Assert.DoesNotContain(containsType, results);

	}

	[Theory]
	// ServerRequest<> is an abstract class, so it shouldn't contain itself.
	[InlineData(typeof(ServerRequest<>), typeof(ServerRequest<>), false)]
	// ListSpeciesRequest is a non-abstract class, so it SHOULD contain itself.
	[InlineData(typeof(ListSpeciesRequest), typeof(ListSpeciesRequest), true)]
	// ListTypesRequest is a non-abstract class that implements ServerRequest<>.
	[InlineData(typeof(ServerRequest<>), typeof(ListTypesRequest), true)]
	// The function should pick up assignable generic functions.
	[InlineData(typeof(ServerRequest<ListTypesResponse>), typeof(ListTypesRequest), true)]
	// It shouldn't get confused with non-assignable types that share the generic type.
	[InlineData(typeof(ServerRequest<ListTypesResponse>), typeof(ListSpeciesRequest), false)]
	public static void GetInstanceableClassesAssignableToTests(Type startingType, Type containsType, bool shouldContain)
	{

		IEnumerable<Type> results =
			TypeUtils.GetInstanceableClassesAssignableTo(startingType);

		if (shouldContain)
			Assert.Contains(containsType, results);
		else
			Assert.DoesNotContain(containsType, results);

	}

	#endregion

}
