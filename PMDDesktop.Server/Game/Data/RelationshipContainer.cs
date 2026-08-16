using PMDDesktop.GameData;
using PMDDesktop.Server.Saving;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Game.Data;

internal class RelationshipContainer
{

	[JsonInclude]
	private Dictionary<Guid, Relationship> relationships = [];

	public Relationship this[Guid guid] { get => GetRelationshipTo(guid); }

	public Relationship this[Character character] { get => this[character.GUID]; }

	private Relationship GetRelationshipTo(Guid guid)
	{

		if (relationships.TryGetValue(guid, out Relationship? found))
			return found;

		Relationship newRelationship = new();
		relationships.Add(guid, newRelationship);

		return newRelationship;

	}

	/// <summary>
	/// This function removes relationships that don't currently point to a loaded character.
	/// </summary>
	public void CleanMissingRelationships(ISaveDataIndexable saves)
	{

		IEnumerable<Guid> missingEnumerable = relationships
			.Select((pair) => pair.Key) // We just want the keys, we don't care about the values.
			.Where(key => saves.GetSave<Character>(key) == null); // Only if the character for this key is missing.

		// This needs to be baked into an array, as the Enumerable will be edited while we're iterating, so this avoids any problems.
		Guid[] missingArray = [.. missingEnumerable];

		foreach (Guid missing in missingArray)
		{

			relationships.Remove(missing);

		}

	}

}
