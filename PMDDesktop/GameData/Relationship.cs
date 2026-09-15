using System.Text.Json.Serialization;

<<<<<<<< HEAD:PMDDesktop.Server.Saving/Data/Relationship.cs
namespace PMDDesktop.Server.Saving.Data;
========
namespace PMDDesktop.GameData;
>>>>>>>> main:PMDDesktop/GameData/Relationship.cs

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
