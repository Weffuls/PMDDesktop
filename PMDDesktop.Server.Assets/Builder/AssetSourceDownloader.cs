using PMDDesktop.Structs;
using PMDDesktop.Utils;

namespace PMDDesktop.Server.Assets.Builder;

public static class AssetSourceDownloader
{

	private static HashSet<string> downloadedHashes = [];

	static AssetSourceDownloader()
	{
		client = new();
	}

	private static readonly HttpClient client;
	public static readonly string saveFolderPath = Path.Combine(AppContext.BaseDirectory, "tmp", "downloads");
	public static bool AlwaysRedownload { get; set; }

	/// <summary>
	/// Downloads a file from the URL and places it with that name in the tmp/downloads folder.
	/// Also provides a visual to the console to inform the user that it's downloading.
	/// </summary>
	/// <param name="client">An http client</param>
	/// <param name="httpUrl">The url you'd like to download from.</param>
	/// <param name="saveName">The file name you'd like to write to.</param>
	/// <returns>The path of the downloaded file.</returns>
	/// <remarks>This will throw if the http response isn't a successful response.</remarks>
	public static async Task<string> DownloadAFile(Uri httpUrl, string saveName)
	{

		Directory.CreateDirectory(saveFolderPath);
		string savePath = Path.Combine(saveFolderPath, saveName);

		if (!downloadedHashes.Add(savePath))
			return savePath;

		if (!AlwaysRedownload && File.Exists(savePath))
		{
			Console.WriteLine($"Skipping download from {httpUrl} because {saveName} already exists.");
			return savePath;
		}

		Console.WriteLine($"Now downloading from: {httpUrl}");

		// Send a request and open a download stream.
		HttpResponseMessage response = await client.GetAsync(httpUrl, HttpCompletionOption.ResponseHeadersRead);
		response.EnsureSuccessStatusCode(); // An error is more useful than a bad download.
		await using Stream downloadStream = await response.Content.ReadAsStreamAsync();

		// Create the directory and create a file to stream to.
		// Note that this is created after we ensure that the response is successful, that way we don't leave blank files on the system.
		using FileStream fileStream = new(savePath, FileMode.Create);

		// Check how big the content length is.
		// It might be null.....
		long bytesTarget = response.Content.Headers.ContentLength ?? -1;
		string targetString;
		if (bytesTarget != -1)
		{
			string size = SizeUtils.ByteSizeToHumanReadable(bytesTarget);
			targetString = $" of {size}";
		}
		else
		{
			targetString = string.Empty;
		}

		// This is a little confusing.
		// We read from the download stream asyncronously.
		// byteCount gets set to the count of bytes that we just read.
		// Once we read 0 bytes, we know we've reached the end of the stream, so we stop looping.
		byte[] buffer = new byte[1024 * 8];
		int byteCount;
		long bytesDownloaded = 0;
		while ((byteCount = await downloadStream.ReadAsync(buffer)) > 0)
		{

			// This copies what we just read to the file.
			await fileStream.WriteAsync(buffer.AsMemory(0, byteCount));

			// This chunk handles updating the progress bar.
			bytesDownloaded += byteCount;
			string downloadedSize = SizeUtils.ByteSizeToHumanReadable(bytesDownloaded);
			OneWayRange progress = new(bytesDownloaded, bytesTarget);
			AssetBuilder.WriteProgress($"{httpUrl}", $"{downloadedSize}{targetString}", progress);

		}

		AssetBuilder.WriteProgress($"{httpUrl}", SizeUtils.ByteSizeToHumanReadable(bytesDownloaded), 1.0f);
		Console.WriteLine();
		Console.WriteLine($"Download successful. Saved as {saveName}");

		// Sleep for a moment.
		// Give servers a chance to rest + avoid ratelimiting when downloading multiple files.
		await Task.Delay(500);

		return savePath;

	}

}
