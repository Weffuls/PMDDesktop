using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

[JsonConverter(typeof(UserRoleConverter))]
public sealed class UserRole
{

	internal UserRole() { }

	internal Dictionary<UserPermission, bool> permissionSettings = [];
	public string Name { get; internal set; } = "Unnamed Role";
	public Guid GUID { get; internal set; } = Guid.NewGuid();
	public bool IsDefaultRole { get => GUID == Guid.Empty; }
	public UserRoleManager? Manager { get; private set; }
	public bool? WritingEnabled { get; private set; }

	public async Task SetName(string newName)
	{

		Name = newName;

		await WriteNewData();

	}

	public async Task SetPermission(UserPermission permission, bool? preference)
	{

		if (preference is bool boolPreference)
			permissionSettings[permission] = boolPreference;
		else
			permissionSettings.Remove(permission);

		if (WritingEnabled is true)
			await WriteNewData();

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
		WritingEnabled = Manager.WritingEnabled;

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

		if (!IsDefaultRole)
			Manager.roleOrder = [.. Manager.roleOrder.Where(guid => guid != GUID)];

		Manager = null;

	}

	/// <summary>
	/// Gets the primary JSON that contains this <see cref="UserRole"/>'s data, located inside the <see cref="UserRole"/> folder.
	/// </summary>
	/// <returns>A path pointing to where the <see cref="UserRole"/>'s json data is/should be stored.</returns>
	internal string GetRoleJSONPath()
	{

		return Path.Combine(AppContext.BaseDirectory, "roles", $"{GUID}.json");

	}

	[ExcludeFromCodeCoverage]
	internal async Task WriteNewData()
	{

		if (Manager is null) // ????? This state doesn't make any sense.
			throw new InvalidOperationException($"{this} shouldn't be writing its data to a file while {nameof(Manager)} is null!");

		if (WritingEnabled is null)
			throw new InvalidOperationException($"{this} shouldn't be writing its data to a file while {nameof(WritingEnabled)} is null!");

		if (WritingEnabled != true)
			return;

		using FileStream jsonFile = File.Create(GetRoleJSONPath());

		await JsonSerializer.SerializeAsync(jsonFile, this, AppInfo.JSON_OPTIONS);

	}

	/// <summary>
	/// <para>Deletes the <see cref="UserRole"/> folder and all *explicitly written* data inside it.</para>
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// <para>This function does not recursive delete. Instead it has a list of items to delete. This approach is taken to minimize data loss in the event that something goes wrong.</para>
	/// </remarks>
	[ExcludeFromCodeCoverage]
	private async Task DeleteRoleFolder()
	{

		if (WritingEnabled is null)
			throw new InvalidOperationException($"{this} shouldn't be deleting its data from a file while {nameof(WritingEnabled)} is null!");

		if (WritingEnabled != true)
			return;

		File.Delete(GetRoleJSONPath());

	}

	public async Task<bool> TryRemoveRole()
	{

		if (Manager is null)
			return false;

		if (IsDefaultRole)
			return false; // Default role cannot be removed manually.

		UserRoleManager oldManager = Manager;

		DetachFromManager();

		await DeleteRoleFolder();

		await oldManager.WriteNewRoleOrder();

		return true;

	}

}
