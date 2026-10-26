using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers;

public class ResponseResultPermissionError(params IEnumerable<UserPermission> permissions) : ResponseResultError
{

	public UserPermission[] Permissions { get; private init; } = [.. permissions];
	public override string GetErrorReason()
	{

		string permList = string.Join(", ", Permissions);

		return Permissions.Length == 1
			? $"You do not have a required permission: {permList}"
			: $"You do not have the required permissions: {permList}";

	}

	public override int GetReturnCode() => 403;

}
