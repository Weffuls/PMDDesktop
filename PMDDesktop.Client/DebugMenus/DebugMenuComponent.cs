#if DEBUG

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMDDesktop.Client.DebugMenus;

internal sealed class DebugMenuComponent : DrawableGameComponent
{

	public static int ELEMENT_PADDING = 3;

	internal DebugMenuComponent(PMDDesktopGame game) : base(game)
	{

		this.PMDGame = game;

		menu = new DebugMenuRoot(this);

		UpdateMenuPathText();

	}

	internal PMDDesktopGame PMDGame { get; init; }

	private bool MenuActive { get; set; }

	private string menuPathText = "???";
	private Stack<IDebugMenu> menuStack = [];
	private IDebugMenu menu;
	private Stack<uint> selectedIndexStack = [];
	private uint selectedIndex;

	public void AddStackMenu(IDebugMenu newMenu)
	{

		menuStack.Push(menu);
		selectedIndexStack.Push(selectedIndex);

		menu = newMenu;
		selectedIndex = 0;

		UpdateMenuPathText();

	}

	public void PopStackMenu()
	{

		menu = menuStack.Pop();
		selectedIndex = selectedIndexStack.Pop();

		UpdateMenuPathText();

	}

	public void UpdateMenuPathText()
	{

		menuPathText = string.Join(" > ", [.. menuStack.Select(menu => menu.Name), menu.Name]);

	}

	public override void Update(GameTime gameTime)
	{

		if (PMDGame.KeyDownThisFrame(Keys.OemTilde))
			MenuActive = !MenuActive;

		if (!MenuActive)
			return;

		if (PMDGame.KeyDownThisFrame(Keys.Down))
			if (selectedIndex < menu.Items.Length - 1)
				selectedIndex += 1;

		if (PMDGame.KeyDownThisFrame(Keys.Up))
			if (selectedIndex > uint.MinValue)
				selectedIndex -= 1;

		if (PMDGame.KeyDownThisFrame(Keys.Enter))
			menu.Items[selectedIndex].Selected();

		if (PMDGame.KeyDownThisFrame(Keys.Back))
			if (menuStack.Count > 0)
				PopStackMenu();

		base.Update(gameTime);

	}

	public override void Draw(GameTime gameTime)
	{

		if (MenuActive)
			DrawMenu();

		base.Draw(gameTime);

	}

	private void DrawMenu()
	{

		PMDGame.SpriteBatch.Begin();

		DrawMenuPath(out Vector2 currentPosition);

		uint drawIndex = 0;
		foreach (IDebugMenuItem item in menu.Items)
		{

			DrawMenuItem(currentPosition, item, drawIndex, out currentPosition);

			++drawIndex;

		}

		PMDGame.SpriteBatch.End();

	}

	private void DrawMenuPath(out Vector2 currentPosition)
	{

		Vector2 pathSize = PMDGame.ContentPool.WonderMailFont.MeasureString(menuPathText);
		Vector2 pathLocation = new(ELEMENT_PADDING, ELEMENT_PADDING);
		Rectangle drawingRectangle = new((int)pathLocation.X, (int)pathLocation.Y, (int)MathF.Ceiling(pathSize.X), (int)MathF.Ceiling(pathSize.Y));

		PMDGame.SpriteBatch.Draw(PMDGame.ContentPool.SolidPixel, drawingRectangle, Color.Black);

		PMDGame.SpriteBatch.DrawString(PMDGame.ContentPool.WonderMailFont, menuPathText, pathLocation, Color.White);

		currentPosition = new(drawingRectangle.Left + ELEMENT_PADDING, drawingRectangle.Bottom + ELEMENT_PADDING);

	}

	private void DrawMenuItem(Vector2 currentPosition, IDebugMenuItem item, uint itemIndex, out Vector2 nextPosition)
	{

		Rectangle backgroundRectangle = new(currentPosition.ToPoint(), (item.Size + new Vector2(ELEMENT_PADDING * 2, ELEMENT_PADDING * 2)).ToPoint());

		PMDGame.SpriteBatch.Draw(PMDGame.ContentPool.SolidPixel, backgroundRectangle, Color.Black);

		item.Draw(itemIndex == selectedIndex, currentPosition + new Vector2(ELEMENT_PADDING, ELEMENT_PADDING));

		nextPosition = new Vector2(currentPosition.X, backgroundRectangle.Bottom + ELEMENT_PADDING);

	}

}

#endif
