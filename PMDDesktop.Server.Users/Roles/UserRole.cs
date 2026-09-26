using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

[JsonConverter(typeof(UserRoleConverter))]
public sealed class UserRole
{

	internal UserRole() { }

	internal Dictionary<UserPermission, bool> permissionSettings = [];
	public string Name {get; set;} = "Unnamed Role";
	public Guid guid = Guid.NewGuid();

	public async Task SetPermission(UserPermission permission, bool? preference)
	{

		if (preference is bool boolPreference)
			permissionSettings[permission] = boolPreference;
		else
			permissionSettings.Remove(permission);

	}

	public bool? GetPermission(UserPermission permission)
	{

		if (permissionSettings.TryGetValue(permission, out bool value))
			return value;

		return null;

	}

}
