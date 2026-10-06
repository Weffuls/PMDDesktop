using System.Security.Cryptography;

namespace PMDDesktop.Server.Users;

public sealed class UserAccessToken
{

	/// <summary>
	/// The time until a <see cref="UserAccessToken"/> expires.
	/// </summary>
	public static readonly TimeSpan TIME_UNTIL_EXPIRE = new(8, 0, 0, 0); // 8 days.

	/// <summary>
	/// The time until the <see cref="ExpiryDate"/> that at which point the token should be refreshed.
	/// </summary>
	public static readonly TimeSpan REFRESH_THRESHOLD = new(7, 0, 0, 0); // 7 Days. // Ideally if you log in once a week, you'll stay logged in.

	/// <summary>
	/// How many random characters should be in our 
	/// </summary>
	public static readonly int RANDOM_CHAR_COUNT = 64;

	public static readonly char[] RANDOM_CHARS = [
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
		'-', '_'
	];

	/// <summary>
	/// Create a new <see cref="UserAccessToken"/> by manually specifying the <see cref="TokenString"/>, <see cref="User"/>, and <see cref="ExpiryDate"/>.
	/// </summary>
	/// <param name="token">The <see cref="TokenString"/> of this <see cref="UserAccessToken"/>.</param>
	/// <param name="forUser">The <see cref="User"/> of this <see cref="UserAccessToken"/>.</param>
	/// <param name="expires">The <see cref="ExpiryDate"/> of this <see cref="UserAccessToken"/>.</param>
	internal UserAccessToken(string token, User forUser, DateTime expires)
	{

		TokenString = token;
		User = forUser;
		ExpiryDate = expires;

	}

	/// <summary>
	/// Create a new, randomized <see cref="UserAccessToken"/> for <paramref name="forUser"/>.
	/// </summary>
	/// <param name="forUser">The <see cref="Users.User"/> that this <see cref="UserAccessToken"/> is intended for.</param>
	/// <returns></returns>
	internal static UserAccessToken CreateNewToken(User forUser)
	{

		string tokenString = RandomNumberGenerator.GetString(RANDOM_CHARS, RANDOM_CHAR_COUNT);

		return new(tokenString, forUser, DateTime.Now + TIME_UNTIL_EXPIRE);

	}

	/// <summary>
	/// The <see cref="DateTime"/> that this <see cref="UserAccessToken"/> expires.
	/// </summary>
	public DateTime ExpiryDate { get; private set; }

	/// <summary>
	/// The unique <see cref="string"/> used to access this <see cref="UserAccessToken"/>.
	/// </summary>
	public string TokenString { get; private set; }

	/// <summary>
	/// The <see cref="Users.User"/> that this <see cref="UserAccessToken"/> points to.
	/// </summary>
	public User User { get; internal set; }

	/// <summary>
	/// Check if this <see cref="UserAccessToken"/> needs a refresh according to <see cref="REFRESH_THRESHOLD"/>.
	/// </summary>
	/// <returns>Returns true if this tokens needs a refresh.</returns>
	public bool NeedsRefresh()
	{

		return DateTime.UtcNow < ExpiryDate - REFRESH_THRESHOLD;

	}

	/// <summary>
	/// Refresh <see cref="ExpiryDate"/> and tell the <see cref="User"/> to save.
	/// </summary>
	/// <returns>Completes once the saving is finished.</returns>
	public async Task RefreshToken()
	{

		ExpiryDate = DateTime.UtcNow + TIME_UNTIL_EXPIRE;

		await User.WriteNewData();

	}

	/// <summary>
	/// Check if this <see cref="UserAccessToken"/> is currently valid. (Not expired)
	/// </summary>
	/// <returns>True if this token can be used.</returns>
	public bool IsTokenValid()
	{

		DateTime now = DateTime.UtcNow;

		return now < ExpiryDate;

	}

	/// <summary>
	/// Tell <see cref="User"/> to revoke this <see cref="UserAccessToken"/>.
	/// </summary>
	/// <returns>Completes task once the token is revoked.</returns>
	public async Task RevokeToken()
	{

		await User.RevokeAccessToken(this);

	}

}
