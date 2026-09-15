using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Saving.Data;


[SaveSubdirectory("character")]
public sealed class Character() : SaveData()
{

	[JsonInclude]
	public string nickname = string.Empty;
	[JsonInclude]
	public RelationshipContainer Relationships { get; private set; } = new();
	[JsonInclude]
	public Personality Personality { get; set; } = new();

}
