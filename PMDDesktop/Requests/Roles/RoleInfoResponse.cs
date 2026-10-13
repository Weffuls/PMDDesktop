namespace PMDDesktop.Requests.Roles;

public sealed class RoleInfoResponse : ServerResponse
{

	/// <summary>
	/// The displayed name of the role.
	/// </summary>
	public required string DisplayName { get; set; }

	/// <summary>
	/// The <see cref="Guid"/> of the role.
	/// </summary>
	public required Guid GUID { get; set; }

	/// <summary>
	/// Dictionary of Permission "data names" (IDs) and whether they're enabled, disabled, or neither.
	/// </summary>
	public required Dictionary<string, bool?>? Permissions { get; set; }

	/// <summary>
	/// <para>The index in which this role is evaluated in the overall role order.</para>
	/// <para>Starting at 0, a number closer to 0 is evaluated before numbers further from 0, thus a higher-priority.</para>
	/// </summary>
	public required int IndexInRoleOrder { get; set; }

	/// <summary>
	/// Is this the "default" role that everyone is assigned and can't be assigned or unassigned?
	/// </summary>
	public required bool IsDefaultRole { get; set; }

}
