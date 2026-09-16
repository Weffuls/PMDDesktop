#if DEBUG

using System;

namespace PMDDesktop.Client.DebugMenus;

internal class RootDebugMenu : IDebugMenu
{

	internal RootDebugMenu(DebugMenuComponent menu)
	{

		this.menu = menu;

		Items = [
			new DebugMenuButton("Send HTTP Requests", () => menu.AddStackMenu(new RequestListDebugMenu(menu)), menu)
		];

	}

	private DebugMenuComponent menu;

	public string Name => "Root";

	public IDebugMenuItem[] Items { get; init; }

}

#endif
