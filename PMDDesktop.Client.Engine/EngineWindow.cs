using CSDL;
using CSDL.Video;

namespace PMDDesktop.Client.Engine;

public class EngineWindow
{

	internal Renderer Renderer { get; private set; }
	protected Window window;
	public EngineGame Engine { get; private set; }
	public bool Transparent { get; private init; }

	internal EngineWindow(EngineGame engine, EngineWindowCreateProperties properties)
	{

		Engine = engine;

		window = new(properties.GetWindowCreateProperties());

		Transparent = properties.Type == EngineWindowCreateProperties.WindowType.Utility;

		Renderer = new(window)
		{
			ScaleMode = ScaleMode.Nearest,
			Vsync = 1,
			ClearColor = Transparent ? Color.Transparent : Color.DarkGrey
		};

	}

	internal void Render()
	{

		Renderer.Clear();

		RenderDebugText();

		Renderer.Present();

	}

	private void RenderDebugText()
	{

		Renderer.DrawColor = Color.Red;

		Renderer.RenderDebugText(0.0f, 0.0f, CSDL.Timer.GetPerformanceCounter().ToString());
		Renderer.RenderDebugText(0.0f, 10.0f, Engine.FPS.ToString("n2"));

	}

}