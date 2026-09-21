using System.Diagnostics.CodeAnalysis;
using PMDDesktop.Exceptions;
using PMDDesktop.Requests;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Utils;

namespace PMDDesktop.Server.RequestHandlers;

internal interface IRequestHandler
{

	private delegate IEndpointConventionBuilder EndpointMapper(string pattern, RequestDelegate requestDelegate);

	static void AddApiPoints(GameServer server)
	{

		IEnumerable<Type> handlerTypes = TypeUtils.GetInstanceableClassesAssignableTo(typeof(IRequestHandler));

		foreach (Type type in handlerTypes)
		{

			IRequestHandler handler =
				((IRequestHandler?)Activator.CreateInstance(type))
				?? throw new InvalidCastException($"Unable to cast instance of {type} to {typeof(IRequestHandler).FullName}");

			Type requestType = handler.GetRequestType();

			if (!ServerRequest<ServerResponse>.ATTRIBUTES.TryGetValue(requestType, out ServerRequestAttribute? attribute))
				throw new MissingAttributeException(requestType, typeof(ServerRequestAttribute));

			EndpointMapper mapper = ServerRequest<ServerResponse>.IsAllQueryProperties(requestType)
				? server.WebApp.MapGet
				: server.WebApp.MapPost;

			mapper(attribute.Path, (context) =>
			{

				return handler.HandleRequest(context, server);

			});

			Console.WriteLine(attribute.Path);

		}

	}

	static bool TryGetUserFromContext(UserManager manager, HttpContext context, [NotNullWhen(true)] out User? user, [NotNullWhen(true)] out UserAccessToken? accessToken)
	{

		string? token = context.Request.Headers.Authorization;

		if (token is null)
		{
			user = null;
			accessToken = null;
			return false;
		}

		return manager.TryUseAccessToken(token, out user, out accessToken);

	}

	/// <summary>
	/// Process the full request as needed.
	/// </summary>
	/// <param name="context">The HttpContext.</param>
	/// <param name="game">Reference to the GameServer to get or set data.</param>
	/// <returns></returns>
	Task HandleRequest(HttpContext context, GameServer game);

	/// <summary>
	/// Should return the type of request object to process.
	/// </summary>
	/// <returns></returns>
	Type GetRequestType();

}
