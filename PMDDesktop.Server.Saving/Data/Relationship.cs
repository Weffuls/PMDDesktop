using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving.Data;

/// <summary>
/// This class holds information about a character's thoughts on another character.
/// It does not neccessarily accurately reflect that character, just how the owning character perceives that character.
/// </summary>
public class Relationship()
{

	[JsonInclude]
	public float Friendship { get; set; } = 0.0f;
	[JsonInclude]
	public float Trust { get; set; } = 0.0f;

}
