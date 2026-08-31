using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

internal abstract class MetaVisual
{

	public required JsonElement groupElement;
	public required IEnumerable<string> groupNames;

	/// <summary>
	/// Does this Pokémon visual seem to align to a gender? For example, if a "{name}-female" exists, then the other one is probably a male, and we need to create two variants.
	/// </summary>
	public GenderAlignment genderAlignment = GenderAlignment.None;
	public required bool isShiny;

}
