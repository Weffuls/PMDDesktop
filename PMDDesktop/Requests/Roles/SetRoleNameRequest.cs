namespace PMDDesktop.Requests.Roles;


[ServerRequest("/api/roles/set-name")]
public sealed class SetRoleNameRequest : ServerRequest<RoleInfoResponse>
{

	/// <summary>
	/// The <see cref="Guid"/> of the role you're trying to set the name of.
	/// </summary>
	public required Guid GUID { get; set; }

	/// <summary>
	/// <para>New name for the new role.</para>
	/// </summary>
	public required string NewName { get; set; }

}
