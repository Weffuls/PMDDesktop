using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.ZipScavenger;

internal class SpriteCollabZip(string path) : Zip(path)
{

	private JsonElement? trackerCache;

	public async Task<JsonElement> GetTrackerJSON()
	{

		if (trackerCache is JsonElement cache)
			return cache;

		ZipArchiveEntry entry = GetEntry("tracker.json");

		Stream stream = await entry.OpenAsync();
		JsonDocument document = await JsonDocument.ParseAsync(stream);

		return (JsonElement)(trackerCache = document.RootElement.Clone());

	}

}
