using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PMDDesktop.Client.DebugMenus;

namespace PMDDesktop.Client;

public class PMDDesktopGame : Game
{
	public GraphicsDeviceManager Graphics { get; private set; }
	public SpriteBatch SpriteBatch { get; private set; }
	public ContentPool ContentPool { get; private set; }
	public KeyboardState LastKeyboardState { get; private set; }
	public KeyboardState KeyboardState { get; private set; }
	public MouseState LastMouseState { get; private set; }
	public MouseState MouseState { get; private set; }

	/// <summary>
	/// <para>When this flag is set, don't update visuals or draw to the screen.</para>
	/// <para>This is used to optimize for when another fullscreen application is running.</para>
	/// </summary>
	public bool BackgroundPowerSavingMode { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
	public PMDDesktopGame()
	{
		Graphics = new GraphicsDeviceManager(this);
		Content.RootDirectory = "Content";
		IsMouseVisible = true;
	}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

	protected override void Initialize()
	{

		base.Initialize();

#if DEBUG
		Components.Add(new DebugMenuComponent(this));
#endif

	}

	protected override void LoadContent()
	{

		SpriteBatch = new SpriteBatch(GraphicsDevice);

		ContentPool = new ContentPool(Content, GraphicsDevice);

	}

	protected override void Update(GameTime gameTime)
	{

		if (!BackgroundPowerSavingMode)
		{

			LastKeyboardState = KeyboardState;
			LastMouseState = MouseState;
			KeyboardState = Keyboard.GetState();
			MouseState = Mouse.GetState();

		}

		base.Update(gameTime);

	}

	protected override void Draw(GameTime gameTime)
	{

		GraphicsDevice.Clear(Color.CornflowerBlue);

		base.Draw(gameTime);

	}

	public bool KeyDownThisFrame(Keys key)
	{

		if (LastKeyboardState.IsKeyUp(key))
			if (KeyboardState.IsKeyDown(key))
				return true;

		return false;

	}

	public bool KeyUpThisFrame(Keys key)
	{

		if (LastKeyboardState.IsKeyDown(key))
			if (KeyboardState.IsKeyUp(key))
				return true;

		return false;

	}

}
