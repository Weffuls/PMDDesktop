namespace PMDDesktop.Requests.Roles;


[ServerRequest("/api/roles/create")]
public sealed class CreateRoleRequest : ServerRequest<RoleInfoResponse>
{

	/// <summary>
	/// <para>Optional name for the new role.</para>
	/// <para>It will be assigned a default name otherwise.</para>
	/// </summary>
	public string? NewName { get; set; }

}
