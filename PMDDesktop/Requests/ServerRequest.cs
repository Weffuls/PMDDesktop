using PMDDesktop.Exceptions;
using System.Reflection;

namespace PMDDesktop.Requests;

public abstract class ServerRequest<T> where T : ServerResponse
{

	static ServerRequest()
	{

		ATTRIBUTES = [];

		IEnumerable<Type> requestTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsClass && !type.IsAbstract && true == type.BaseType?.IsGenericType && typeof(ServerRequest<>).IsAssignableFrom(type.BaseType?.GetGenericTypeDefinition()));

		Dictionary<string, Type> endpointCollisionChecks = [];

		foreach (Type type in requestTypes)
		{

			// Check for server request attributes.
			// Each server request should have one.

			ServerRequestAttribute requestAttribute = type.GetCustomAttribute<ServerRequestAttribute>()
			?? throw new MissingAttributeException(type, typeof(ServerRequestAttribute));

			if (endpointCollisionChecks.TryGetValue(requestAttribute.Path, out Type? clashing))
				throw new DuplicateAttributeDataException(type, clashing, requestAttribute.Path, typeof(ServerRequestAttribute));

			ATTRIBUTES.Add(type, requestAttribute);
			endpointCollisionChecks.Add(requestAttribute.Path, type);

		}

	}

	public static readonly Dictionary<Type, ServerRequestAttribute> ATTRIBUTES;

	private static string GetEndpointInternal(Type type)
	{

		return ATTRIBUTES[type].Path;

	}

	public string GetEndpoint()
	{

		return ServerRequest<T>.GetEndpointInternal(GetType());

	}

}
