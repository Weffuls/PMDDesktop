using Microsoft.AspNetCore.Identity;
using PMDDesktop.Server.Users.Roles;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users;

[JsonConverter(typeof(UserConverter))]
public sealed class User : IUserHierarchyComparable
{

	/// <summary>
	/// <see cref="PasswordHasher{User}"/> to use to hash passwords.
	/// </summary>
	private static PasswordHasher<User> PASSWORD_HASHER = new();

	/// <summary>
	/// The name of the file that <see cref="User"/> data should be serialized to.
	/// </summary>
	public static readonly string USER_FILE_NAME = "user.json";

	/// <summary>
	/// A list of items to delete when deleting a <see cref="User"/>.
	/// </summary>
	private static string[] DELETE_USER_FOLDER_ITEMS = [USER_FILE_NAME, "picture.jpg", "picture.png"];

	/// <summary>
	/// <para>Limit of <see cref="UserAccessToken"/>s assigned to one <see cref="User"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para>This is supposed to help combat a senario where a user creates millions of access tokens to intentionally fill up a hard drive or system memory.</para>
	/// <para>Downside is that it prevents an honest user from having a large number of legitimate connections.</para>
	/// </remarks>
	public static readonly int ACCESS_TOKEN_LIMIT = 20;

	[JsonConstructor]
	internal User()
	{

	}

	/// <summary>
	/// <para>A string representing a <see cref="User"/>'s name.</para>
	/// </summary>
	[JsonInclude]
	public string Name { get; internal set; } = string.Empty;

	/// <summary>
	/// <para>A string to use for logging in. Should only ever contain [a-z], [0-9] and (-)</para>
	/// <para>Does not neccessarily match <see cref="Name"/> in most senarios.</para>
	/// <para>Should be unique.</para>
	/// </summary>
	[JsonInclude]
	public string? LoginHandle { get; internal set; }

	/// <summary>
	/// <para>Hashed password used to log in.</para>
	/// <para>In the event that it is null, access to this account cannot be granted via password.</para>
	/// </summary>
	[JsonInclude]
	internal string? HashedPassword { get; set; }

	/// <summary>
	/// <para>A unique <see cref="Guid"/> for this <see cref="User"/>. Stays consistant even when the <see cref="User"/>'s handle changes.</para>
	/// <para>This should be used for long-term references to a specific <see cref="User"/>.</para>
	/// </summary>
	[JsonIgnore]
	public Guid GUID { get; internal set; } = Guid.NewGuid();

	/// <summary>
	/// Attached <see cref="UserManager"/> that controls this <see cref="User"/>.
	/// </summary>
	[JsonIgnore]
	public UserManager? Manager { get; private set; }

	/// <summary>
	/// Is/Was <see cref="UserManager.WritingEnabled"/> on the last used or currently active <see cref="UserManager"/>.
	/// </summary>
	public bool? WritingEnabled { get => Manager is not null ? Manager.WritingEnabled : field; private set; }

	/// <summary>
	/// Specifies if this user is affected by <see cref="UserPermission.AlwaysForAdmin"/>.
	/// </summary>
	public bool IsAdmin { get; internal set; }

	/// <summary>
	/// <para>List of <see cref="UserAccessToken"/>s currently belonging to this <see cref="User"/>.</para>
	/// <para>May include expired <see cref="UserAccessToken"/>s.</para>
	/// </summary>
	internal List<UserAccessToken> accessTokens = [];

	/// <summary>
	/// <para>Collection of <see cref="UserRole"/>s assigned to this user by their <see cref="Guid"/>s. Order does not matter, use <see cref="UserRoleManager.roleOrder"/> for order instead.</para>
	/// <para>Also, <see cref="UserRoleManager.DefaultRole"/> is not included in this. It is implied to always be assigned.</para>
	/// </summary>
	internal HashSet<Guid> roles = [];

	#region Name

	/// <summary>
	/// Set the <see cref="Name"/> of this <see cref="User"/>.
	/// </summary>
	/// <param name="newName">The new name of the <see cref="User"/>.</param>
	/// <returns>Completes task once changes are saved.</returns>
	public async Task SetName(string newName)
	{

		Name = newName;

		if (Manager is not null)
			await WriteNewData();

	}

	#endregion

	#region Login Handle & Password

	/// <summary>
	/// Salt and hash the user's password, then store it in HashedPassword.
	/// </summary>
	/// <param name="newPassword">The plaintext password to be hashed.</param>
	/// <returns>Completes task after saving completes.</returns>
	public async Task SetPassword(string? newPassword)
	{

		if (newPassword is string passwordText)
			HashedPassword = PASSWORD_HASHER.HashPassword(this, passwordText);
		else
			HashedPassword = null;

		if (Manager is not null)
			await WriteNewData();

	}

	/// <summary>
	/// Try to set <see cref="LoginHandle"/> to the input. Will fail if it is not unique.
	/// </summary>
	/// <param name="newHandle">The string to change the new handle to.</param>
	/// <returns>True if the handle was changed and saved.</returns>
	public async Task<bool> TrySetLoginHandle(string? newHandle)
	{

		// Handle is not the same.
		if (newHandle == LoginHandle)
			return false;

		// Handle is free.
		if (newHandle is not null && Manager is not null && Manager.loginHandles.ContainsKey(newHandle))
			return false;

		DetachLoginHandle();

		LoginHandle = newHandle;

		AttachLoginHandle();

		if (Manager is not null)
			await WriteNewData();

		return true;

	}

	/// <summary>
	/// Add the current <see cref="LoginHandle"/> to <see cref="UserManager.loginHandles"/>.
	/// </summary>
	private void AttachLoginHandle()
	{

		if (LoginHandle is null)
			return;

		Manager?.loginHandles.Add(LoginHandle, this);

	}

	/// <summary>
	/// Remove the current <see cref="LoginHandle"/> from <see cref="UserManager.loginHandles"/>.
	/// </summary>
	private void DetachLoginHandle()
	{

		if (LoginHandle is null)
			return;

		Manager?.loginHandles.Remove(LoginHandle);

	}

	/// <summary>
	/// <para>Check that <paramref name="plainText"/> hashes to the same password stored in the user.</para>
	/// <para>May also rehash the password if evidence is shown that it needs a rehash.</para>
	/// </summary>
	/// <param name="plainText">The password input by the user.</param>
	/// <returns>True if the password matched.</returns>
	public async Task<bool> VerifyPassword(string plainText)
	{

		if (HashedPassword is null)
		{

			return false;

		}

		PasswordVerificationResult result = PASSWORD_HASHER.VerifyHashedPassword(this, HashedPassword, plainText);

		if (result == PasswordVerificationResult.SuccessRehashNeeded)
		{

			await SetPassword(plainText);

		}

		return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;

	}

	public bool HasPassword()
	{

		return HashedPassword is not null;

	}

	#endregion

	#region Saving

	/// <summary>
	/// Return the filesystem folder that should contain all of this user's data.
	/// </summary>
	/// <returns>A path pointing to the user's folder.</returns>
	/// <remarks>This path is created using the user's GUID.</remarks>
	internal string GetUserFolder()
	{

		return Path.Combine(AppContext.BaseDirectory, "users", GUID.ToString());

	}

	/// <summary>
	/// Gets the primary JSON that contains this user's data, located inside the user folder.
	/// </summary>
	/// <returns>A path pointing to where the user's json data is/should be stored.</returns>
	public string GetUserJSONPath()
	{

		return Path.Combine(GetUserFolder(), USER_FILE_NAME);

	}

	/// <summary>
	/// Saves all data relating to this <see cref="User"/> object.
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

		foreach (UserAccessToken token in accessTokens)
			if (DateTime.UtcNow >= token.ExpiryDate)
				await RevokeAccessToken(token, false);

		if (WritingEnabled != true)
			return;

		Directory.CreateDirectory(GetUserFolder());
		using FileStream jsonFile = File.Create(GetUserJSONPath());

		await JsonSerializer.SerializeAsync(jsonFile, this, AppInfo.JSON_OPTIONS);

	}

	/// <summary>
	/// <para>Deletes the user folder and all *explicitly written* data inside it.</para>
	/// </summary>
	/// <returns>Completes task once folder is deleted... or after it fails to delete due to having unmanaged filesystem entries.</returns>
	/// <remarks>
	/// <para>This function does not recursive delete. Instead it has a list of items to delete. This approach is taken to minimize data loss in the event that something goes wrong.</para>
	/// </remarks>
	[ExcludeFromCodeCoverage]
	private async Task DeleteUserFolder()
	{

		if (WritingEnabled is null)
			throw new InvalidOperationException($"{this} shouldn't be deleting its data from the filesystem while {nameof(WritingEnabled)} is null!");

		if (WritingEnabled != true)
			return;

		foreach (string item in DELETE_USER_FOLDER_ITEMS)
		{

			string path = Path.Join(GetUserFolder(), item);

			if (File.Exists(path))
				File.Delete(path);

		}

		if (Directory.GetFileSystemEntries(GetUserFolder()).Length == 0)
			Directory.Delete(GetUserFolder(), false);
		else
			Console.WriteLine($"CANNOT DELETE ALL OF {this}'s user folder!!!");

	}

	#endregion

	#region Generic Object Overrides

	public override bool Equals(object? obj)
	{

		if (obj is User user)
		{

			return user.GUID == GUID;

		}

		return false;

	}

	public override int GetHashCode()
	{
		return HashCode.Combine(GUID);
	}

	#endregion

	#region Access Tokens

	/// <summary>
	/// Create a new <see cref="UserAccessToken"/> for this user.
	/// </summary>
	/// <returns>Returns the new <see cref="UserAccessToken"/>.</returns>
	/// <remarks>
	/// Will remove existing access tokens if <see cref="ACCESS_TOKEN_LIMIT"/> is exceeded.
	/// </remarks>
	public async Task<UserAccessToken> CreateAccessToken()
	{

		while (accessTokens.Count >= ACCESS_TOKEN_LIMIT)
			await TryRevokeOldestAccessToken(false);

		UserAccessToken token = UserAccessToken.CreateNewToken(this);

		accessTokens.Add(token);

		AttachAccessToken(token);

		if (Manager is not null)
			await WriteNewData();

		return token;

	}

	/// <summary>
	/// Attaches <paramref name="token"/> to the attached <see cref="UserManager"/>.
	/// </summary>
	/// <param name="token">The <see cref="UserAccessToken"/> to attach to <see cref="Manager"/>.</param>
	/// <remarks>
	/// Attaching will be skipped if <see cref="Manager"/> is null, so this is safe to call when detached.
	/// </remarks>
	private void AttachAccessToken(UserAccessToken token)
	{

		// if (accessTokens.Any(existing => existing.TokenString == token.TokenString))
		// 	throw new Exception($"The token: {token.TokenString} is already in {this}'s {nameof(accessTokens)}.");

		Manager?.accessTokens.Add(token.TokenString, token);

	}

	/// <summary>
	/// Revokes <paramref name="token"/>, removes it from the <see cref="UserManager"/>, then (optionally) saves.
	/// </summary>
	/// <param name="token">The <see cref="UserAccessToken"/> to revoke.</param>
	/// <param name="writeAfterwards">Should save afterwards? Always skipped if <see cref="Manager"/> is null.</param>
	/// <returns>Completes task when saving completes.</returns>
	internal async Task RevokeAccessToken(UserAccessToken token, bool writeAfterwards)
	{

		accessTokens.Remove(token);

		DetachAccessToken(token);

		if (Manager is not null && writeAfterwards)
			await WriteNewData();

	}

	/// <summary>
	/// Revokes <paramref name="token"/>, removes it from the <see cref="UserManager"/>, then saves.
	/// </summary>
	/// <param name="token">The <see cref="UserAccessToken"/> to revoke.</param>
	/// <returns>Completes task when saving completes.</returns>
	public async Task RevokeAccessToken(UserAccessToken token)
	{

		await RevokeAccessToken(token, true);

	}

	/// <summary>
	/// Detaches <paramref name="token"/> from the attached <see cref="UserManager"/>.
	/// </summary>
	/// <param name="token">The <see cref="UserAccessToken"/> to detach to <see cref="Manager"/>.</param>
	/// <remarks>
	/// Detaching will be skipped if <see cref="Manager"/> is null, so this is safe to call when detached.
	/// </remarks>
	private void DetachAccessToken(UserAccessToken token)
	{

		Manager?.accessTokens.Remove(token.TokenString);

	}

	/// <summary>
	/// Attempt to revoke the oldest <see cref="UserAccessToken"/> managed by this <see cref="User"/>. Can optionally save afterwards.
	/// </summary>
	/// <param name="writeAfterwards">Should save afterwards?</param>
	/// <returns>Returns true if an access token was revoked.</returns>
	private async Task<bool> TryRevokeOldestAccessToken(bool writeAfterwards = true)
	{

		UserAccessToken? oldest = null;

		foreach (UserAccessToken token in accessTokens)
		{

			oldest ??= token;

			if (oldest.ExpiryDate > token.ExpiryDate)
				oldest = token;

		}

		if (oldest is null)
			return false;

		await RevokeAccessToken(oldest, writeAfterwards);
		return true;

	}

	#endregion

	#region Manager Attaching

	/// <summary>
	/// Attach to a <see cref="UserManager"/> to provide this <see cref="User"/>'s data to the <paramref name="manager"/>.
	/// </summary>
	/// <param name="manager">The <see cref="UserManager"/> to attach to.</param>
	/// <exception cref="InvalidOperationException">Throws if already attached to a <see cref="UserManager"/>.</exception>
	internal void AttachToManager(UserManager manager)
	{

		if (Manager != null)
			throw new InvalidOperationException($"This existing value of {nameof(Manager)} is not null. Cannot attach while already attached. Does this call to {nameof(AttachToManager)} need to be skipped, or does {nameof(DetachFromManager)} need to be called first?");

		Manager = manager;

		Manager.guids.Add(GUID, this);

		AttachLoginHandle();

		foreach (UserAccessToken token in accessTokens)
			AttachAccessToken(token);

	}

	/// <summary>
	/// Detach from the attached <see cref="UserManager"/> to remove this <see cref="User"/>'s data from the attached <see cref="UserManager"/>.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws if not currently attached to a <see cref="UserManager"/>.</exception>
	/// <remarks>
	/// This doesn't really need to be called on server shutdown. It is called internally when deleting a user.
	/// </remarks>
	internal void DetachFromManager()
	{

		if (Manager == null)
			throw new NullReferenceException($"{nameof(Manager)} is null. Cannot detach from nothing. Does this call to {nameof(DetachFromManager)} need to be skipped?");

		if (Manager.guids[GUID] != this)
			throw new InvalidOperationException($"{this} was not inside {Manager.guids} with key {GUID}.");

		Manager.guids.Remove(GUID);

		DetachLoginHandle();

		foreach (UserAccessToken token in accessTokens)
			DetachAccessToken(token);

		WritingEnabled = Manager.WritingEnabled;
		Manager = null;

	}

	public bool IsAlive() => Manager is not null;

	#endregion

	#region User Deletion

	/// <summary>
	/// <para>Try to remove this <see cref="User"/> from its <see cref="UserManager"/>.</para>
	/// <para>If the assigned <see cref="UserManager"/> has <see cref="UserManager.WritingEnabled"/>, then this function will also attempt to delete the user's folder from the filesystem, so that it will not be restored in future sessions.</para>
	/// </summary>
	/// <remarks>
	/// If <see cref="UserManager.WritingEnabled"/> is enabled, this function may still throw if an IO-related error occurs.
	/// </remarks>
	public async Task<bool> TryRemoveUser()
	{

		if (Manager is null)
			return false;

		DetachFromManager();

		await DeleteUserFolder();

		return true;

	}

	#endregion

	#region Role/Permission Management

	/// <summary>
	/// Try to add a <see cref="UserRole"/> to this <see cref="User"/>. Allows the <see cref="User"/> to be influenced by the <see cref="UserRole"/>'s <see cref="UserPermission"/>s.
	/// </summary>
	/// <param name="role">The <see cref="UserRole"/> to add to the <see cref="User"/>.</param>
	/// <returns>Returns true if it was added successfully.</returns>
	/// <remarks>
	/// <para>Primarily fails when the <see cref="UserRole"/> is already on the <see cref="User"/>.</para>
	/// <para>This also saves the user.</para>
	/// </remarks>
	public async Task<bool> TryAddRole(UserRole role)
	{

		if (!roles.Add(role.GUID))
			return false;

		await WriteNewData();

		return true;

	}

	/// <summary>
	/// Try to remove a <see cref="UserRole"/> from this <see cref="User"/>. Stops the <see cref="User"/> from being influenced by the <see cref="UserRole"/>'s <see cref="UserPermission"/>s.
	/// </summary>
	/// <param name="role">The <see cref="UserRole"/> to remove from the <see cref="User"/>.</param>
	/// <returns>Returns true if it was removed successfully.</returns>
	/// <remarks>
	/// <para>Primarily fails when the <see cref="User"/> does not have the <see cref="UserRole"/>.</para>
	/// <para>This also saves the user.</para>
	/// </remarks>
	public async Task<bool> TryRemoveRole(UserRole role)
	{

		if (!roles.Remove(role.GUID))
			return false;

		await WriteNewData();

		return true;

	}

	/// <summary>
	/// Check if the <see cref="User"/> has <paramref name="role"/> assigned to it.
	/// </summary>
	/// <param name="role">The <see cref="UserRole"/> to check for.</param>
	/// <returns>True if this user is assigned this role.</returns>
	public bool HasRole(UserRole role)
	{

		return roles.Contains(role.GUID);

	}

	/// <summary>
	/// Does this <see cref="User"/>, looking at the assigned <see cref="UserRole"/>s with respect to <see cref="UserRoleManager"/>'s role order, have <paramref name="permission"/>?
	/// </summary>
	/// <param name="permission"></param>
	/// <returns>Returns true if this user has the <paramref name="permission"/>.</returns>
	/// <exception cref="NullReferenceException">Throws if not currently attached to a <see cref="UserManager"/>.</exception>
	public bool HasPermission(UserPermission permission)
	{

		if (Manager is null)
			throw new NullReferenceException($"Can't check if {this} has a permission if {nameof(Manager)} is null!");

		// Is this an admin permission?
		// Checked after Manager because we shouldn't be checking for permissions on a detached user anyways.
		if (permission.AlwaysForAdmin && IsAdmin)
			return true;

		foreach (UserRole role in Manager.RoleManager)
		{

			if (!HasRole(role))
				continue;

			bool? newState = role.GetPermission(permission);

			if (newState is bool final)
				return final;

		}

		return false;

	}

	/// <summary>
	/// Set whether this <see cref="User"/> is an admin and is affected by <see cref="UserPermission.AlwaysForAdmin"/>.
	/// </summary>
	/// <param name="newState">Whether or not this <see cref="User"/> should be an admin.</param>
	/// <returns>Completes task once changes are saved.</returns>
	public async Task SetAdmin(bool newState)
	{

		IsAdmin = newState;

		if (Manager is not null)
			await WriteNewData();

	}

	/// <summary>
	/// <para>Returns true if this <see cref="User"/> could edit <paramref name="target"/> based on <see cref="IUserHierarchyComparable"/>, given that it has the correct permissions.</para>
	/// <para>Before you let an edit happen, ensure the user has both the correct permission and a higher hierarchy.</para>
	/// </summary>
	/// <returns>True if this user is higher than <paramref name="target"/>.</returns>
	public bool HigherThan(IUserHierarchyComparable target)
	{

		return GetHierarchyPosition() < target.GetHierarchyPosition();

	}

	public int GetHierarchyPosition()
	{

		if (Manager is null)
			throw new NullReferenceException($"Can't check where {this} is in the hierarchy if {nameof(Manager)} is null!");

		// Checked after Manager because we shouldn't be checking for hierarchy on a detached user anyways.
		if (IsAdmin)
			return -1;

		foreach (UserRole role in Manager.RoleManager)
		{

			if (!HasRole(role))
				continue;

			return role.GetCurrentOrderIndex();

		}

		throw new InvalidOperationException($"Not sure how this happened, but we didn't find a role that {this} matched with. Not even the default role.");

	}

	#endregion

}
