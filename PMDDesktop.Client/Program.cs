namespace PMDDesktop.Client;

internal static class Program
{

	private static void Main(string[] args)
	{

		using var game = new PMDDesktopGame();
		game.Run();

	}

}
