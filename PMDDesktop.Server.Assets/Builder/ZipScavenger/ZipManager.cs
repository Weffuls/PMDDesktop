namespace PMDDesktop.Server.Assets.Builder.ZipScavenger;

internal static class ZipManager
{

	public static async Task<PokeApiZip> GetPokeApiZip()
	{

		string path = await AssetSourceDownloader.DownloadAFile(new("https://github.com/PokeAPI/api-data/archive/refs/heads/master.zip"), "pokeapi.zip");

		return new(path);

	}

	public static async Task<SpriteCollabZip> GetSpriteCollabZip()
	{

		string path = await AssetSourceDownloader.DownloadAFile(new("https://github.com/PMDCollab/SpriteCollab/archive/refs/heads/master.zip"), "spriteCollab.zip");

		return new(path);

	}

}
