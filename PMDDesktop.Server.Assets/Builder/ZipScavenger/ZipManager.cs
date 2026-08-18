namespace PMDDesktop.Server.Assets.Builder.ZipScavenger;

internal static class ZipManager
{

	public static async Task<PokeApiZip> GetPokeApiZip()
	{

		Uri uri = new("https://github.com/PokeAPI/api-data/archive/refs/heads/master.zip");
		string fileName = "pokeapi.zip";

		string path = await AssetSourceDownloader.DownloadAFile(uri, fileName);

		try
		{
			return new(path);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		Console.WriteLine("Failed to read zip, downloading one more time; assuming the file is corrupted.");
		File.Delete(path);
		await AssetSourceDownloader.DownloadAFile(uri, fileName);
		return new(path);

	}

	public static async Task<SpriteCollabZip> GetSpriteCollabZip()
	{

		Uri uri = new("https://github.com/PMDCollab/SpriteCollab/archive/refs/heads/master.zip");
		string fileName = "spriteCollab.zip";

		string path = await AssetSourceDownloader.DownloadAFile(uri, fileName);

		try
		{
			return new(path);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}

		Console.WriteLine("Failed to read zip, downloading one more time; assuming the file is corrupted.");
		File.Delete(path);
		await AssetSourceDownloader.DownloadAFile(uri, fileName);
		return new(path);

	}

}
