using PMDDesktop.Server.Saving;

namespace PMDDesktop.Server;

[SaveSubdirectory("users")]
internal sealed class User : SaveData
{

	public string displayName;
	public string loginHandle;

	private User(string handle) : base()
	{

		loginHandle = handle;
		displayName = handle.ToUpper();

	}

}
