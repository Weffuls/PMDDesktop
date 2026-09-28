using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

[JsonConverter(typeof(UserRoleConverter))]
public sealed class UserRole
{

	internal UserRole() { }

	internal Dictionary<UserPermission, bool> permissionSettings = [];
	public string Name { get; set; } = "Unnamed Role";
	public Guid GUID { get; internal set; } = Guid.NewGuid();
	public bool IsDefaultRole { get => GUID == Guid.Empty; }
	public UserRoleManager? Manager {get; private set;}

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

	internal static UserRole CreateDefaultRole()
	{

		UserRole role = new()
		{
			GUID = Guid.Empty,
			Name = "Default (Everyone)"
		};

		foreach (UserPermission permission in UserPermission.ALL.Where(perm => perm.EnabledForDefault))
			role.permissionSettings.Add(permission, true);

		return role;

	}

	internal void AttachToManager(UserRoleManager newManager)
	{

		if (Manager != null)
			throw new InvalidOperationException($"This existing value of {nameof(Manager)} is not null. Cannot attach while already attached. Does this call to {nameof(AttachToManager)} need to be skipped, or does {nameof(DetachFromManager)} need to be called first?");

		Manager = newManager;

		newManager.guids.Add(GUID, this);

		if (!IsDefaultRole)
			newManager.roleOrder = [.. newManager.roleOrder, GUID];

	}

	internal void DetachFromManager()
	{

		if (Manager == null)
			throw new NullReferenceException($"{nameof(Manager)} is null. Cannot detach from nothing. Does this call to {nameof(DetachFromManager)} need to be skipped?");

		if (Manager.guids[GUID] != this)
			throw new InvalidOperationException($"{this} was not inside {Manager.guids} with key {GUID}.");

		Manager.guids.Remove(GUID);

		Manager = null;

	}

}
