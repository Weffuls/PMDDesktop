using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PMDDesktop.Client;

public class PMDDesktopGame : Game
{
	private GraphicsDeviceManager graphics;
	private SpriteBatch spriteBatch;
	public ContentPool ContentPool { get; private set; }

	public PMDDesktopGame()
	{
		graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}

	protected override void Initialize()
	{

		// TODO: Add your initialization logic here

		base.Initialize();

	}

	protected override void LoadContent()
	{

		spriteBatch = new SpriteBatch(GraphicsDevice);

		ContentPool = new ContentPool(Content);

	}

	protected override void Update(GameTime gameTime)
	{

		// TODO: Add your update logic here

		base.Update(gameTime);

	}

	protected override void Draw(GameTime gameTime)
	{

		GraphicsDevice.Clear(Color.CornflowerBlue);

		spriteBatch.Begin();

		spriteBatch.DrawString(ContentPool.WonderMailFont, "Weffuls", Vector2.One, Color.White);

		spriteBatch.End();

		base.Draw(gameTime);

	}

}
