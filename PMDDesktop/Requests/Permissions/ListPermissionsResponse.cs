namespace PMDDesktop.Requests.Permissions;

public sealed class ListPermissionsResponse : ServerResponse
{

	/// <summary>
	/// All currently existing roles, presented in order.
	/// </summary>
	public required PermissionInfoResponse[] Permissions { get; set; }

}
