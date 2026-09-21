using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PMDDesktop.Server.Users;

public sealed class User
{

	private static PasswordHasher<User> PASSWORD_HASHER = new();

	/// <summary>
	/// A list
	/// </summary>
	private static string[] DELETE_USER_FOLDER_ITEMS = ["user.json", "picture.jpg", "picture.png"];

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
	public string? LoginHandle { get; private set; }

	/// <summary>
	/// <para>Hashed password used to log in.</para>
	/// <para>In the event that it is null, access to this account cannot be granted via password.</para>
	/// </summary>
	[JsonInclude]
	private string? HashedPassword { get; set; }

	/// <summary>
	/// <para>A unique <see cref="Guid"/> for each <see cref="User"/>. Stays consistant even when the <see cref="User"/>'s handle changes.</para>
	/// <para>This should be used for long-term references to a specific <see cref="User"/>.</para>
	/// </summary>
	[JsonIgnore]
	public Guid GUID { get; internal set; } = Guid.NewGuid();

	/// <summary>
	///
	/// </summary>
	[JsonIgnore]
	public UserManager? Manager {get; private set;}

	public bool? WritingEnabled {get; private set;}

	private List<UserAccessToken> accessTokens = [];

	/// <summary>
	/// Salt and hash the user's password, then store it in HashedPassword.
	/// </summary>
	/// <param name="plainText"></param>
	/// <returns></returns>
	public async Task SetPassword(string plainText)
	{

		HashedPassword = PASSWORD_HASHER.HashPassword(this, plainText);

	}

	/// <summary>
	///
	/// </summary>
	/// <param name="newHandle"></param>
	/// <returns></returns>
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

		return true;

	}

	private void AttachLoginHandle()
	{

		if (LoginHandle is null)
			return;

		Manager?.loginHandles.Add(LoginHandle, this);

	}

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

		return Path.Combine(GetUserFolder(), "user.json");

	}

	[ExcludeFromCodeCoverage]
	internal async Task WriteNewData()
	{

		if (Manager is null) // ????? This state doesn't make any sense.
			throw new InvalidOperationException($"{this} shouldn't be writing its data without being attached to a {nameof(Manager)}.");

		if (WritingEnabled is null)
			throw new InvalidOperationException($"{this} shouldn't be deleting its data while {WritingEnabled} is null!");

		if (WritingEnabled != true)
			return;

		throw new NotImplementedException();

	}

	/// <summary>
	/// <para>Deletes the user folder and all *explicitly written* data inside it.</para>
	/// </summary>
	/// <returns></returns>
	/// <remarks>
	/// <para>This function does not recursive delete. Instead it has a list of items to delete. This approach is taken to minimize data loss in the event that something goes wrong.</para>
	/// </remarks>
	[ExcludeFromCodeCoverage]
	private async Task DeleteUserFolder()
	{

		if (WritingEnabled is null)
			throw new InvalidOperationException($"{this} shouldn't be deleting its data while {WritingEnabled} is null!");

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

	public async Task<UserAccessToken> CreateAccessToken()
	{

		while (accessTokens.Count >= ACCESS_TOKEN_LIMIT)
			await TryRevokeOldestAccessToken();

		UserAccessToken token = UserAccessToken.CreateNewToken(this);

		accessTokens.Add(token);

		AttachAccessToken(token);

		return token;

	}

	private void AttachAccessToken(UserAccessToken token)
	{

		// if (accessTokens.Any(existing => existing.TokenString == token.TokenString))
		// 	throw new Exception($"The token: {token.TokenString} is already in {this}'s {nameof(accessTokens)}.");

		Manager?.accessTokens.Add(token.TokenString, token);

	}

	public async Task RevokeAccessToken(UserAccessToken token)
	{

		accessTokens.Remove(token);

		DetachAccessToken(token);

	}

	private void DetachAccessToken(UserAccessToken token)
	{

		Manager?.accessTokens.Remove(token.TokenString);

	}

	private async Task<bool> TryRevokeOldestAccessToken()
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

		await RevokeAccessToken(oldest);
		return true;

	}

	internal void AttachToManager(UserManager manager)
	{

		Manager = manager;
		WritingEnabled = manager.WritingEnabled;

		Manager.guids.Add(GUID, this);

		AttachLoginHandle();

		foreach (UserAccessToken token in accessTokens)
			AttachAccessToken(token);

	}

	internal void DetachFromManager()
	{

		if (Manager==null)
			throw new NullReferenceException($"{nameof(Manager)} is null. Cannot detach from nothing. Does this call to {nameof(DetachFromManager)} need to be skipped?");

		if (Manager.guids[GUID] != this)
			throw new InvalidOperationException($"{this} was not inside {Manager.guids} with key {GUID}.");

		Manager.guids.Remove(GUID);

		DetachLoginHandle();

		foreach (UserAccessToken token in accessTokens)
			DetachAccessToken(token);

		Manager = null;

	}

	public bool IsAlive() => Manager is not null;

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

}
