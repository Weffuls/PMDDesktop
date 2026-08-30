using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// This file contains extremely specialized helper functions for BuildSpecies relating to the PMDCollab's SpriteCollab.
/// They are delegated here to help BuildSpecies be easy to follow the flow of.
/// </summary>
internal static class SpriteCollabUtils
{

	public static async Task<JsonElement> GetSpeciesTop(string index, SpriteCollabZip zip)
	{

		JsonElement tracker = await zip.GetTrackerJSON();

		return tracker.GetProperty(index);

	}

	public static async Task<IEnumerable<MetaVisual>> GetAllVisuals(string index, SpriteCollabZip zip)
	{

		// TODO: Do something.

		throw new NotImplementedException();

	}

}
