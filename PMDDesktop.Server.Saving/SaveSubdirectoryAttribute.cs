namespace PMDDesktop.Server.Saving;

/// <summary>
/// <para>This attribute sets the name of the subfolder that SaveData is written to and loaded from.</para>
/// <para>This should consist of lowercase "a-z" and "-", for example "species" or "inventories".</para>
/// </summary>
/// <param name="subdirectoryName">The subdirectory to save to, for example "species" or "inventories".</param>
[AttributeUsage(AttributeTargets.Class)]
public class SaveSubdirectoryAttribute(string subdirectoryName) : Attribute
{

	/// <summary>
	/// <para>The subdirectory name, should consist of lowercase "a-z" and "-".</para>
	/// <para>Represents a single folder/directory.</para>
	/// </summary>
	internal string subdirectoryName = subdirectoryName.ToLowerInvariant();

}
