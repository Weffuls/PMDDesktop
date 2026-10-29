using System.Security.Cryptography;
using PMDDesktop.Client.Engine;

namespace PMDDesktop.Client;

internal static class Program
{

	private static int Main(string[] args)
	{

		EngineGame engine = new();

		CreateDemoWindow(engine);
		CreateDemoWindow(engine);
		CreateDemoWindow(engine);
		CreateDemoWindow(engine);
		CreateDemoWindow(engine);

		engine.Run();

		return 0;

	}

	private static void CreateDemoWindow(EngineGame engine)
	{

		EngineWindowCreateProperties properties = new()
		{
			Type = (EngineWindowCreateProperties.WindowType)Math.Abs(Random.Shared.Next() % 3)
		};

		engine.CreateWindow(properties);

	}

}
