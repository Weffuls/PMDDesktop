using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users.Roles;

[JsonConverter(typeof(UserRoleConverter))]
public sealed class UserRole
{

	internal UserRole() { }

	/// <summary>
	/// <para>A <see cref="Dictionary{UserPermission, bool}"/> containing granted/revoked <see cref="UserPermission"/>s.</para>
	/// <para>An entry is true if this <see cref="UserRole"/> grants that <see cref="UserPermission"/>, and false if it revokes the <see cref="UserPermission"/>.</para>
	/// <para>A non-existant entry states that the <see cref="UserPermission"/> is unaffected.</para>
	/// </summary>
	internal Dictionary<UserPermission, bool> permissionSettings = [];

	/// <summary>
	/// <para>A string representing a <see cref="UserRole"/>'s name.</para>
	/// </summary>
	public string Name { get; internal set; } = "Unnamed Role";

	/// <summary>
	/// <para>A unique <see cref="Guid"/> for this <see cref="UserRole"/>.</para>
	/// <para>This should be used for long-term references to a specific <see cref="UserRole"/>.</para>
	/// </summary>
	public Guid GUID { get; internal set; } = Guid.NewGuid();

	/// <summary>
	/// Weather this is the "default role."
	/// </summary>
	/// <remarks>
	/// Currently just checks if <see cref="GUID"/> matches <see cref="Guid.Empty"/>.
	/// </remarks>
	public bool IsDefaultRole { get => GUID == Guid.Empty; }

	/// <summary>
	/// Attached <see cref="UserRoleManager"/> that controls this <see cref="UserRole"/>.
	/// </summary>
	public UserRoleManager? Manager { get; private set; }

	/// <summary>
	/// Is/Was <see cref="UserManager.WritingEnabled"/> on the last used or currently active <see cref="UserRoleManager"/>.
	/// </summary>
	public bool? WritingEnabled { get; private set; }

	/// <summary>
	/// Set the <see cref="Name"/> of this <see cref="UserRole"/>.
	/// </summary>
	/// <param name="newName">The new name of this <see cref="UserRole"/>.</param>
	/// <returns>Completes the task once the <see cref="UserRole"/> has saved.</returns>
	public async Task SetName(string newName)
	{

		Name = newName;

		await WriteNewData();

	}

	/// <summary>
	/// <para>Set the state of <paramref name="permission"/> in this <see cref="UserRole"/>.</para>
	/// <para>Pass true to permit <paramref name="permission"/>, false to revoke it, and null to not affect it.</para>
	/// </summary>
	/// <param name="permission">The <see cref="UserPermission"/> to modify.</param>
	/// <param name="preference">True if permission should be granted, False is permission should be revoked, null if permission should be unaffected.</param>
	/// <returns>Completes the task once the <see cref="UserRole"/> has saved.</returns>
	public async Task SetPermission(UserPermission permission, bool? preference)
	{

		if (preference is bool boolPreference)
			permissionSettings[permission] = boolPreference;
		else
			permissionSettings.Remove(permission);

		if (WritingEnabled is true)
			await WriteNewData();

	}

	/// <summary>
	/// Get the state of <paramref name="permission"/> in this <see cref="UserRole"/>.
	/// </summary>
	/// <param name="permission">The <see cref="UserPermission"/> to check.</param>
	/// <returns>True if permission is granted, False is permission is revoked, null if permission is unaffected.</returns>
	public bool? GetPermission(UserPermission permission)
	{

		if (permissionSettings.TryGetValue(permission, out bool value))
			return value;

		return null;

	}

	/// <summary>
	/// Create a new <see cref="UserRole"/> with <see cref="Guid.Empty"/> and the <see cref="UserPermission"/>s marked as <see cref="UserPermission.EnabledForDefault"/>.
	/// </summary>
	/// <returns>A new <see cref="UserRole"/> configured to be the default role.</returns>
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

	/// <summary>
	/// Attach to a <see cref="UserRoleManager"/> to provide this <see cref="UserRole"/>'s data to the <paramref name="newManager"/>.
	/// </summary>
	/// <param name="newManager">The <see cref="UserRoleManager"/> to attach to.</param>
	/// <exception cref="InvalidOperationException">Throws if already attached to a <see cref="UserRoleManager"/>.</exception>
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

	/// <summary>
	/// Detach from the attached <see cref="UserRoleManager"/> to remove this <see cref="UserRole"/>'s data from the attached <see cref="UserRoleManager"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws if not currently attached to a <see cref="UserRoleManager"/>.</exception>
	/// <remarks>
	/// This doesn't really need to be called on server shutdown. It is called internally when deleting a <see cref="UserRole"/>.
	/// </remarks>
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

	/// <summary>
	/// Saves all data relating to this <see cref="UserRole"/> object.
	/// </summary>
	/// <returns>Task completes once data is written.</returns>
	/// <exception cref="InvalidOperationException">If the <see cref="Manager"/> or if <see cref="WritingEnabled"/> is null.</exception>
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

	/// <summary>
	/// <para>Try to remove this <see cref="UserRole"/> from its <see cref="UserRoleManager"/>.</para>
	/// <para>If the assigned <see cref="UserRoleManager"/> has <see cref="UserRoleManager.WritingEnabled"/>, then this function will also attempt to delete the role's file from the filesystem, so that it will not be restored in future sessions.</para>
	/// </summary>
	/// <remarks>
	/// If <see cref="UserRoleManager.WritingEnabled"/> is enabled, this function may still throw if an IO-related error occurs.
	/// </remarks>
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
