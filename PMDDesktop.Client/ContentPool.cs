using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace PMDDesktop.Client;

public sealed class ContentPool : IDisposable
{

	internal ContentPool(ContentManager contentManager, GraphicsDevice graphicsDevice)
	{

		this.contentManager = contentManager;
		this.graphicsDevice = graphicsDevice;

		// TODO: Make this credit more clear; somewhere in the Client UI.
		WonderMailFont = contentManager.Load<SpriteFont>("Fonts/WonderMail"); // Wonder Mail font by ShinxHijinx

		SolidPixel = new Texture2D(graphicsDevice, 1, 1);
		SolidPixel.SetData([Color.White]);

	}

	private ContentManager contentManager;
	private GraphicsDevice graphicsDevice;

	public Texture2D SolidPixel { get; init; }

	public SpriteFont WonderMailFont { get; init; }

	public void Dispose()
	{

		SolidPixel.Dispose();

	}

}
