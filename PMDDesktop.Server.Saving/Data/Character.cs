using PMDDesktop.GameData;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving.Data;


[SaveSubdirectory("character")]
public sealed class Character() : SaveData()
{

	[JsonInclude]
	public string Nickname { get; set; } = string.Empty;
	[JsonInclude]
	public RelationshipContainer Relationships { get; private set; } = new();
	[JsonInclude]
	public Personality Personality { get; set; } = new();

}
