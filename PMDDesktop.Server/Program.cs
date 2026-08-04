using PMDDesktop.Server.Assets.Builder;
using PMDDesktop.Server.Game;
using System.CommandLine;

namespace PMDDesktop.Server;

/// <summary>
/// 
/// </summary>
internal static class Program
{

	/// <summary>
	/// Entry point for the program.
	/// </summary>
	/// <param name="args">Arguments passed through the commandline.</param>
	private static async Task<int> Main(string[] args)
	{

		RootCommand root = new("Server for PMDDesktop");

		// [program] start
		AddStartCommand(root);

		// [program] assets
		AddAssetCommands(root);

		InvocationConfiguration configuration = new()
		{
			ProcessTerminationTimeout = null
		};

		return await root.Parse(args).InvokeAsync(configuration);

	}

	private static void AddStartCommand(RootCommand root)
	{

		Command startCommand = new("start", "Start the server.");

		startCommand.SetAction(async (result) =>
		{

			GameServer server = await GameServer.CreateAndLoadFiles();

			await server.Run();

		});

		root.Add(startCommand);

	}

	private static void AddAssetCommands(RootCommand root)
	{

		// [program] assets [command]
		Command assetsCommand = new("assets", "Tools for checking/listing assets or building them.");
		root.Add(assetsCommand);

		AddAssetBuildSubcommand(assetsCommand);

		// [program] assets list
		Command listCommand = new("list", "List loaded assets");
		assetsCommand.Add(listCommand);

		// [program] assets check <ID>
		Command checkCommand = new("check", "Print details on an asset by ID.");
		assetsCommand.Add(checkCommand);
		Argument<string> checkArgument = new("ID");
		checkCommand.Add(checkArgument);

	}

	private static void AddAssetBuildSubcommand(Command command)
	{

		// [program] assets build --redownload
		Option<bool> buildRedownloadOption = new("--redownload", "-r")
		{
			Description = "Specify to always redownload files, even if they're already present in the file system.",
			Arity = ArgumentArity.ZeroOrOne,
			Recursive = true
		};

		// [program] assets build
		Command buildCommand = new("build", "Build assets by downloading their source GitHub and formatting them for use.")
		{
			buildRedownloadOption
		};
		buildCommand.SetAction(async (result) =>
		{

			AssetSourceDownloader.AlwaysRedownload = result.GetValue(buildRedownloadOption);

			return await AssetBuilder.RunAllSteps();

		});

		AddAssetBuildSteps(command);

		command.Add(buildCommand);

	}

	private static void AddAssetBuildSteps(Command command)
	{



	}

}
