#if DEBUG

namespace PMDDesktop.Client.DebugMenus;

internal interface IDebugMenu
{

	string Name { get; }

	IDebugMenuItem[] Items { get; }

}

#endif
