#if DEBUG

using System;

namespace PMDDesktop.Client.DebugMenus;

internal class DebugMenuRoot : IDebugMenu
{

	internal DebugMenuRoot(DebugMenuComponent menu)
	{

		this.menu = menu;

		Items = [
			new DebugMenuButton("Test", () => Console.WriteLine("Test pressed!"), menu),
			new DebugMenuButton("Testing 234", () => Console.WriteLine("Testing 234 pressed!"), menu),
			new DebugMenuButton("Recursive Open", () => menu.AddStackMenu(new DebugMenuRoot(menu)), menu)
		];

	}

	private DebugMenuComponent menu;

	public string Name => "Root";

	public IDebugMenuItem[] Items { get; init; }

}

#endif
