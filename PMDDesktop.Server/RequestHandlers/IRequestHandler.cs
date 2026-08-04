using PMDDesktop.Exceptions;
using PMDDesktop.Requests;
using PMDDesktop.Server.Game;

namespace PMDDesktop.Server.RequestHandlers;

internal interface IRequestHandler
{

	static void AddApiPoints(GameServer server)
	{

		IEnumerable<Type> handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IRequestHandler)));

		foreach (Type type in handlerTypes)
		{

			IRequestHandler handler =
				((IRequestHandler?)Activator.CreateInstance(type))
				?? throw new InvalidCastException($"Unable to cast instance of {type} to {typeof(IRequestHandler).FullName}");

			Type requestType = type.BaseType?.GetGenericArguments()[0] // Indexing the first item is... weird. If there's a better way to do this, I'm all on board.
				?? throw new Exception($"Couldn't get generic argument from {type}");

			if (!ServerRequest<ServerResponse>.ATTRIBUTES.TryGetValue(requestType, out ServerRequestAttribute? attribute))
				throw new MissingAttributeException(requestType, typeof(ServerRequestAttribute));

			server.WebApp.MapPost(attribute.Path, (context) =>
			{

				return handler.HandleRequest(context, server);

			});

			Console.WriteLine(attribute.Path);

		}

	}

	Task HandleRequest(HttpContext context, GameServer game);

}
