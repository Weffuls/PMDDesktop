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

	public static IEnumerable<Type> GetInstanceableClasses()
	{

		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsClass && !type.IsAbstract);

	}

	/// <summary>
	/// Returns all
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

}
