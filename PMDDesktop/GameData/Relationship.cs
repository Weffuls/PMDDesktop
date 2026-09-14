using System.Text.Json.Serialization;

namespace PMDDesktop.GameData;

/// <summary>
/// This class holds information about a character's thoughts on another character.
/// It does not neccessarily accurately reflect that character, just how the owning character perceives that character.
/// </summary>
public class Relationship()
{

	[JsonInclude]
	public float friendship = 0.0f;
	[JsonInclude]
	public float trust = 0.0f;

}
