namespace PMDDesktop.Requests.Permissions;

public sealed class PermissionInfoResponse : ServerResponse
{

	/// <summary>
	/// This is the name used to identify this permission. Should be sent back exactly when talking about this permission with the server.
	/// </summary>
	public required string DataName { get; set; }

	/// <summary>
	/// The name that's best shown to users.
	/// </summary>
	public required string FriendlyName { get; set; }

	/// <summary>
	/// A description explaining what the permission does.
	/// </summary>
	public required string FriendlyDescription { get; set; }

}
