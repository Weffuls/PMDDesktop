using PMDDesktop.Exceptions;
using PMDDesktop.Utils;
using System.Reflection;
using System.Text.Json.Serialization.Metadata;

namespace PMDDesktop.Requests;

public abstract class ServerRequest<T> where T : ServerResponse
{

	static ServerRequest()
	{

		ATTRIBUTES = [];

		IEnumerable<Type> requestTypes = TypeUtils.GetInstanceableClassesAssignableTo(typeof(ServerRequest<>));

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

	/// <summary>
	/// When the input ServerRequest is serialized, will it be made up of entirely QueryParameters?
	/// </summary>
	/// <param name="type">The ServerRequest to check.</param>
	/// <returns>True if all ROOT serialzed properties are QueryParameters.</returns>
	public static bool IsAllQueryProperties(Type type)
	{

		IEnumerable<JsonPropertyInfo> properties = AppInfo.NETWORK_JSON_OPTIONS.GetSerializationProperties(type);

		// Lambda gets any properties that aren't query types.
		// Then, if we get one, we return the opposite.
		return !properties.Any((property) => property.AttributeProvider == null || !property.AttributeProvider.IsDefined(typeof(QueryParameterAttribute), true));

	}

	public string GetEndpoint()
	{

		return ServerRequest<T>.GetEndpointInternal(GetType());

	}

}
