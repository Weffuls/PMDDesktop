using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace PMDDesktop.Server.Users;

public sealed class UserManager() : IEnumerable<User>, IUserIndexable
{

	/// <summary>
	/// <para>Does this <see cref="UserManager"/> actually write to files?</para>
	/// <para>If this is false, file writes to the <b>filesystem/disk</b> will be skipped.</para>
	/// </summary>
	/// <remarks>
	/// This is property as "false" is very useful in unit testing. Should probably be "true" during runtime.
	/// </remarks>
	public bool WritingEnabled { get; private set; } = false;

	/// <summary>
	/// <para>This is a dictionary with all the <see cref="User"/>s mapped to their <see cref="User.GUID"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para>This dictionary's entries are not managed by the <see cref="UserManager"/>, but instead by the <see cref="User"/>s as they are attached/detached. <b>Do not let <see cref="UserManager"/> write to this dictionary.</b></para>
	/// </remarks>
	internal Dictionary<Guid, User> guids = [];

	/// <summary>
	/// <para>This is a dictionary with all the <see cref="User.LoginHandle"/>s mapped to their <see cref="User"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para>This dictionary's entries are not managed by the <see cref="UserManager"/>, but instead by the <see cref="User"/>s inside <see cref="guids"/>. <b>Do not let <see cref="UserManager"/> write to this dictionary.</b></para>
	/// </remarks>
	internal Dictionary<string, User> loginHandles = [];

	/// <summary>
	/// <para>This is a dictionary with all the <see cref="UserAccessToken"/>s mapped to their <see cref="UserAccessToken.TokenString"/>.</para>
	/// </summary>
	/// <remarks>
	/// <para>This dictionary's entries are not managed by the <see cref="UserManager"/>, but instead by the <see cref="User"/>s inside <see cref="guids"/>. <b>Do not let <see cref="UserManager"/> write to this dictionary.</b></para>
	/// </remarks>
	internal Dictionary<string, UserAccessToken> accessTokens = [];

	/// <summary>
	/// Loads from the 'users' and 'roles' directories, and enables <see cref="WritingEnabled"/>.
	/// </summary>
	/// <remarks>
	/// This can throw under many, many circumstances. If it does, cancel all operations and make sure the error is conveyed to the user.
	/// </remarks>
	[ExcludeFromCodeCoverage]
	public async Task LoadFromFilesAndEnableWriting()
	{

		// Since Users check the WritingEnabled value, we need to set this before loading.
		WritingEnabled = true;

		await LoadAllUserData();

		await LoadAllRoleData();

	}

	/// <summary>
	/// This manages creating <see cref="User"/> objects by loading their data from the "users" folder.
	/// </summary>
	[ExcludeFromCodeCoverage]
	private async Task LoadAllUserData()
	{

		string userRootDirPath = Path.Combine(AppContext.BaseDirectory, "users");

		if (!Directory.Exists(userRootDirPath))
			Directory.CreateDirectory(userRootDirPath);

		foreach (string subDirPath in Directory.EnumerateDirectories(userRootDirPath))
		{

			string guidParsable = Path.GetFileName(subDirPath);

			// This is not okay. All user folders should have GUIDs are their names.
			if (!Guid.TryParse(guidParsable, out Guid loadedGUID))
				throw new Exception($"Couldn't parse {guidParsable} as a GUID at {subDirPath}");

			string userFilePath = Path.Join(subDirPath, User.USER_FILE_NAME);

			using FileStream readStream = File.OpenRead(userFilePath);

			await LoadAndAddUserJson(readStream, loadedGUID);

		}

	}

	/// <summary>
	/// Internal function for loading a JSON stream and adding the resulting <see cref="User"/> to this <see cref="UserManager"/>.
	/// </summary>
	/// <param name="stream">A JSON stream with <see cref="User"/> data inside.</param>
	/// <returns></returns>
	/// <remarks>
	/// This can be safely unit tested, as it takes in a stream instead of reading from a file.
	/// </remarks>
	internal async Task<User> LoadAndAddUserJson(Stream stream, Guid guid)
	{

		User deserialized = JsonSerializer.Deserialize<User>(stream, AppInfo.JSON_OPTIONS)
					?? throw new Exception($"Deserialized save data from {stream} was null.");

		deserialized.GUID = guid; // GUID will be randomized by default, we need to load the previous GUID.

		deserialized.AttachToManager(this);

		return deserialized;

	}

	/// <summary>
	/// This manages creating <see cref="Role"/> objects by loading their data from the "users" folder.
	/// </summary>
	[ExcludeFromCodeCoverage]
	private async Task LoadAllRoleData()
	{

		// Not yet implemented.

	}

	public IEnumerator<User> GetEnumerator()
	{
		return guids.Values.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	/// <summary>
	/// <para>Try to get a <see cref="User"/> using the string of an access token. Never use this function for any purpose other than authorization.</para>
	/// <para>This function respects token expiration, and may clear tokens that have expired. It may also refresh a token's expiration if a certain amount of time has passed.</para>
	/// <para>If this <see cref="UserManager"/> is <see cref="WritingEnabled"/>, this function may write to the filesystem/disk to update token information.</para>
	/// </summary>
	/// <param name="tokenString">The token string corresponding to a <see cref="UserAccessToken"/>, but not an error if it doesn't.</param>
	/// <returns></returns>
	public bool TryUseAccessToken(string tokenString, [NotNullWhen(true)] out User? user)
	{

		return TryUseAccessToken(tokenString, out user, out _);

	}

	/// <summary>
	/// <para>Try to get a <see cref="User"/> using the string of an access token. Never use this function for any purpose other than authorization.</para>
	/// <para>This function respects token expiration, and may clear tokens that have expired. It may also refresh a token's expiration if a certain amount of time has passed.</para>
	/// <para>If this <see cref="UserManager"/> is <see cref="WritingEnabled"/>, this function may write to the filesystem/disk to update token information.</para>
	/// </summary>
	/// <param name="tokenString">The token string corresponding to a <see cref="UserAccessToken"/>, but not an error if it doesn't.</param>
	/// <returns>True if access should be allowed, otherwise false.</returns>
	/// <remarks>Sometimes, <paramref name="token"/> and <paramref name="user"/> may not be null, even if false was returned. In this case, do not give access, but you may inspect the results.</remarks>
	public bool TryUseAccessToken(string tokenString, [NotNullWhen(true)] out User? user, [NotNullWhen(true)] out UserAccessToken? token)
	{

		accessTokens.TryGetValue(tokenString, out token);
		user = token?.User;

		if (token is null || user is null)
			return false;

		if (!token.IsTokenValid())
			return false;

		return true;

	}

	public bool TryGetUser(Guid GUID, [NotNullWhen(true)] out User? user)
	{

		return guids.TryGetValue(GUID, out user);

	}

	public bool TryGetUserByLoginHandle(string handle, [NotNullWhen(true)] out User? user)
	{

		return loginHandles.TryGetValue(handle, out user);

	}

	public async Task<User?> TryCreateUser(UserCreationOptions options)
	{

		User user = new()
		{
			Name = options.DisplayName
		};

		if (options.LoginHandle is not null)
			if (!await user.TrySetLoginHandle(options.LoginHandle))
				return null;
		if (options.PlainTextPassword is not null)
			await user.SetPassword(options.PlainTextPassword);

		user.AttachToManager(this);
		return user;

	}

}
