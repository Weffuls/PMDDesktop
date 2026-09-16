using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PMDDesktop.Client;

public class ContentPool
{

	internal ContentPool(ContentManager contentManager)
	{

		// TODO: Make this credit more clear; somewhere in the Client UI.
		WonderMailFont = contentManager.Load<SpriteFont>("Fonts/WonderMail"); // Wonder Mail font by ShinxHijinx

	}

	private ContentManager contentManager;

	public SpriteFont WonderMailFont { get; init; }

}
