using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace PMDDesktop.Utils;

public static class TypeUtils
{

	public static IEnumerable<Type> GetBaseTypes(this Type type, bool includeSelf = false)
	{

		if (includeSelf)
			yield return type;

		while (type.BaseType != null)
		{

			type = type.BaseType;

			yield return type;

		}

	}

	/// <summary>
	/// Returns all classes that are not abstract.
	/// </summary>
	/// <returns></returns>
	/// <remarks>Note: You may not have access to the constructors of these classes.</remarks>
	public static IEnumerable<Type> GetInstanceableClasses()
	{

		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsClass && !type.IsAbstract);

	}

	/// <summary>
	/// Returns all classes that are instanceable and belong to a certain type, including generically.
	/// </summary>
	/// <param name="assignableTo"></param>
	/// <returns></returns>
	public static IEnumerable<Type> GetInstanceableClassesAssignableTo(Type assignableTo)
	{

		return GetInstanceableClasses()
			.Where((type) =>
				type.IsAssignableTo(assignableTo) ||
				type.GetBaseTypes(true)
					.Any((baseType) =>
						baseType.IsGenericType &&
						baseType.GetGenericTypeDefinition() == assignableTo
			));

	}

	/// <summary>
	/// Returns all members in a Type that have an Attribute.
	/// </summary>
	/// <typeparam name="T">The Attribute to look for.</typeparam>
	/// <param name="type">The Type to look at.</param>
	/// <returns>An enumerable to get all members in the type with attribute.</returns>
	public static IEnumerable<MemberInfo> GetMembersWithAttribute<T>(this Type type) where T : Attribute
	{

		return type.GetMembers().Where((member) => member.GetCustomAttribute<T>() != null);

	}

	/// <summary>
	/// Gets all properties that will be serialized by a JsonSerializerOptions. Also ensures a TypeInfoResolver is present.
	/// </summary>
	/// <param name="options">The options to use to get type information.</param>
	/// <param name="type">The type to examine.</param>
	/// <returns>All properties that will be serialized.</returns>
	public static IEnumerable<JsonPropertyInfo> GetSerializationProperties(this JsonSerializerOptions options, Type type)
	{

		// Just in case. Many times we won't have one yet.
		options.TypeInfoResolver ??= new DefaultJsonTypeInfoResolver();

		JsonTypeInfo typeInfo = options.GetTypeInfo(type);

		return typeInfo.Properties;

	}

}
