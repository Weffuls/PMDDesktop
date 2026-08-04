namespace PMDDesktop.Server.Saving;

/// <summary>
/// This attribute sets the name of the subfolder that SaveData is written to and loaded from.
/// To keep this styled, this should consist of lowercase "a-z" and "-", but should theoretically work for anything anyways.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class SaveSubdirectoryAttribute(string subdirectoryName) : Attribute
{

	internal string subdirectoryName = subdirectoryName.ToLowerInvariant();

}
