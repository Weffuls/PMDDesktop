using PMDDesktop.Server.Assets.Builder.BuildSteps;
using PMDDesktop.Structs;

namespace PMDDesktop.Server.Assets.Builder;

public static class AssetBuilder
{

	public static readonly BuildStep[] BUILD_STEPS =
	[
		new("types", BuildTypes.StartBuildStep),
		new("species", BuildSpecies.StartBuildStep),
		new("visuals", BuildVisuals.StartBuildStep)
	];

	public static async Task<int> RunAllSteps(bool writeAssets = true)
	{

		AssetManager assets = [];

		foreach (BuildStep step in BUILD_STEPS)
		{
			await step.onExecute(assets);
		}

		if (!writeAssets)
			await assets.WriteAllAssets();

		return 0;

	}

	public static void DeleteAssetsFolder()
	{

		Directory.Delete(AssetLocation.GetAssetsDirectory(), true);

	}

	/// <summary>
	/// Writes a progress bar, replacing the current console line. The progress is displayed using a blue background.
	/// </summary>
	/// <param name="taskName">The name of the task. Will be left-justified, may be cropped.</param>
	/// <param name="details">The details of the task, usually something like "20.0MiB / 30.0MiB" or anything else to help communicate progress.</param>
	/// <param name="progress">A OneWayRange stating the progress of the task.</param>
	internal static void WriteProgress(string taskName, string details, OneWayRange progress)
	{

		Console.SetCursorPosition(0, Console.CursorTop);

		int width = Console.WindowWidth;
		int spaceRemaining = width - 5;

		// Details are more important, so they're trimmed first.
		int detailLength = Math.Min(spaceRemaining, details.Length);
		string trimmedDetails = details[..detailLength];
		spaceRemaining -= detailLength;

		// Then we do the same thing to the name.
		int nameLength = Math.Min(spaceRemaining, taskName.Length);
		string trimmedName = taskName[..nameLength];
		spaceRemaining -= nameLength;

		// Now we create the padding if we have leftover room.
		string padding = new(' ', spaceRemaining + 1);

		string fullString = $"[ {trimmedName}{padding}{trimmedDetails} ]";

		int litCharacters = (int)(progress * width + 0.5f);

		string litString = fullString[0..litCharacters];
		string unlitString = fullString[litCharacters..^0];

		Console.ForegroundColor = ConsoleColor.White;
		Console.BackgroundColor = ConsoleColor.Blue;
		Console.Write(litString);

		Console.BackgroundColor = ConsoleColor.DarkBlue;
		Console.Write(unlitString);
		Console.ResetColor();

	}

}
