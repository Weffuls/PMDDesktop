using PMDDesktop.Server.Saving;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Game.Data;


[SaveSubdirectory("character")]
internal sealed class Character() : GameData()
{

	[JsonInclude]
	public string nickname = string.Empty;
	[JsonInclude]
	public RelationshipContainer Relationships { get; private set; } = new();
	[JsonInclude]
	public Personality Personality { get; set; } = new();

}
