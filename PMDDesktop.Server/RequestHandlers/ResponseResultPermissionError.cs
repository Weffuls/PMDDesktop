using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers;

/// <summary>
/// A <see cref="ResponseResultError"/> that states that the user does not have the required permission(s) to perform an action.
/// </summary>
/// <param name="permissions">A list of missing <see cref="UserPermission"/>s that will be shown to the user.</param>
public class ResponseResultPermissionError(params IEnumerable<UserPermission> permissions) : ResponseResultError
{

	/// <summary>
	/// A array of <see cref="UserPermission"/>s that were missing and resulted in this error.
	/// </summary>
	public UserPermission[] Permissions { get; private init; } = [.. permissions];

	public override string GetErrorReason()
	{

		string permList = string.Join(", ", Permissions);

		return Permissions.Length == 1
			? $"You do not have a required permission: {permList}"
			: $"You do not have the required permissions: {permList}";

	}

	public override int GetStatusCode() => 403;

}
