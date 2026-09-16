#if DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PMDDesktop.Exceptions;
using PMDDesktop.Requests;
using PMDDesktop.Utils;

namespace PMDDesktop.Client.DebugMenus;

internal class RequestListDebugMenu : IDebugMenu
{

	internal RequestListDebugMenu(DebugMenuComponent menu)
	{

		this.menu = menu;

		IEnumerable<Type> requestTypes = TypeUtils.GetInstanceableClassesAssignableTo(typeof(ServerRequest<>));

		Items = [.. requestTypes.Select(type => new DebugMenuButton(RequestTypeToButtonText(type), () => {}, menu))];

	}

	private static string RequestTypeToButtonText(Type type)
	{
				
		ServerRequestAttribute attribute = type.GetCustomAttribute<ServerRequestAttribute>()
			?? throw new MissingAttributeException(type, typeof(ServerRequestAttribute));

		return $"{type}\n{attribute.Path}";

	}

	private DebugMenuComponent menu;

	public string Name => "Server Requests";

	public IDebugMenuItem[] Items { get; init; }

}

#endif