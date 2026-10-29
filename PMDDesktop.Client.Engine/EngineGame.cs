using CSDL;

namespace PMDDesktop.Client.Engine;

public sealed class EngineGame : Game
{

	private List<EngineWindow> windows = [];

	// Delta Time Calculations
	private ulong lastFrameTime;
	public float DeltaTime { get; private set; }
	public float FPS { get; private set; }

	static EngineGame()
	{

		// ily wayland but not having a set position means we gotta use x11
		if (OperatingSystem.IsLinux())
			Hints.VideoDriver.Set("x11");

		Hints.RenderDriver.Set("opengl");

	}

	public EngineGame()
	{

	}

	public EngineWindow CreateWindow(EngineWindowCreateProperties properties)
	{

		EngineWindow newWindow = new(this, properties);

		windows.Add(newWindow);

		return newWindow;

	}

	protected override AppResult OnInit(string[] args)
	{

		return AppResult.Continue;

	}

	protected override AppResult OnIterate()
	{

		// Delta Time Calculation
		ulong frameTime = CSDL.Timer.GetTicksNs();
		ulong frameTimeDifference = frameTime - lastFrameTime;
		lastFrameTime = frameTime;
		DeltaTime = (float)frameTimeDifference / CSDL.Timer.NSPerSecond;
		FPS = 1.0f / DeltaTime;

		foreach (EngineWindow window in windows)
		{
			window.Render();
		}

		return AppResult.Continue;

	}

	protected override AppResult OnEvent(ref Event @event)
	{
		return AppResult.Continue;
	}

	protected override void OnQuit(AppResult result)
	{

	}

}
