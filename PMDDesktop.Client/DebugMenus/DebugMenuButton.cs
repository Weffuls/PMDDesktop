#if DEBUG

using Microsoft.Xna.Framework;
using System;

namespace PMDDesktop.Client.DebugMenus;

internal class DebugMenuButton(string text, Action action, DebugMenuComponent menu) : IDebugMenuItem
{

	private string text = text;
	private Action action = action;

	public Vector2 Size { get; init; } = menu.PMDGame.ContentPool.WonderMailFont.MeasureString(text);

	public void Draw(bool isHighlighted, Vector2 position)
	{
		menu.PMDGame.SpriteBatch.DrawString(menu.PMDGame.ContentPool.WonderMailFont, text, position, isHighlighted ? Color.Green : Color.White);
	}

	public void Selected()
	{
		action();
	}
}

#endif
