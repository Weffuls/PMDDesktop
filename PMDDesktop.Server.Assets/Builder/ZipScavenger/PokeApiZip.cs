using System.IO.Compression;

namespace PMDDesktop.Server.Assets.Builder.ZipScavenger;

internal class PokeApiZip(string path) : Zip(path)
{

	private IEnumerable<ZipArchiveEntry> EnumerateIntoDirectory(string pathToDirectory, string pathFromSubdirectoryToFile)
	{

		if (!pathToDirectory.EndsWith('/'))
			pathToDirectory += '/';

		if (!pathFromSubdirectoryToFile.StartsWith('/'))
			pathFromSubdirectoryToFile = '/' + pathFromSubdirectoryToFile;

		return EnumerateEntries(new($@"^{pathToDirectory}[^/]*{pathFromSubdirectoryToFile}$"));

	}

	public IEnumerable<ZipArchiveEntry> EnumerateSpecies()
	{

		string left = @"api-data-master/data/api/v2/pokemon-species/";
		string right = @"/index.json";

		return EnumerateIntoDirectory(left, right);

	}

	public IEnumerable<ZipArchiveEntry> EnumerateTypes()
	{

		string left = @"api-data-master/data/api/v2/type";
		string right = @"/index.json";

		foreach (ZipArchiveEntry entry in EnumerateIntoDirectory(left, right))
		{

			if (entry.FullName.Contains("100"))
				continue;

			yield return entry;

		}

		yield break;

	}

}
