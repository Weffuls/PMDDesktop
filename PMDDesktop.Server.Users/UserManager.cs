using System.Collections;
using System.Diagnostics.CodeAnalysis;

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

	public User? GetUser(Guid GUID)
	{

		TryGetUser(GUID, out User? user);

		return user;

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
