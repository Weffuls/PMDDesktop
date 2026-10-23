namespace PMDDesktop.Requests.Roles;

public sealed class ListRolesResponse : ServerJsonResponse
{

	/// <summary>
	/// All currently existing roles, presented in order.
	/// </summary>
	public required RoleInfoResponse[] Roles { get; set; }

}
