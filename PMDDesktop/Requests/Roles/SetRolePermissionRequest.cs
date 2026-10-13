namespace PMDDesktop.Requests.Roles;

[ServerRequest("/api/roles/set-permission")]
public sealed class SetRolePermissionRequest : ServerRequest<RoleInfoResponse>
{

	/// <summary>
	/// The <see cref="Guid"/> of the role you're trying to edit.
	/// </summary>
	public required Guid GUID { get; set; }

	/// <summary>
	/// The "data name" of the permission you're trying to edit.
	/// </summary>
	public required string PermissionDataName { get; set; }

	/// <summary>
	/// <para>The value you'd like to set the permission to.</para>
	/// <para>True allows this permission for this role, false denies it, and null makes the role not affect this permission.</para>
	/// <para>When evaluating permissions for a user, they check the highest role they have assigned with a non-null permission value for that permission.</para>
	/// </summary>
	public required bool? NewPermissionValue { get; set; }

}
