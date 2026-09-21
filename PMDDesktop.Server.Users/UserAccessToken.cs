using System.Security.Cryptography;

namespace PMDDesktop.Server.Users;

public sealed class UserAccessToken
{

	public static readonly TimeSpan TIME_UNTIL_EXPIRE = new(8, 0, 0, 0); // 8 days.
	public static readonly TimeSpan REFRESH_THRESHOLD = new(7, 0, 0, 0); // 7 Days. // Ideally if you log in once a week, you'll stay logged in.

	public static readonly int RANDOM_CHAR_COUNT = 64;

	public static readonly char[] RANDOM_CHARS = [
		'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
		'-', '_'
	];

	internal UserAccessToken(string token, User forUser, DateTime expires)
	{

		TokenString = token;
		User = forUser;
		ExpiryDate = expires;

	}

	internal static UserAccessToken CreateNewToken(User forUser)
	{

		string tokenString = RandomNumberGenerator.GetString(RANDOM_CHARS, RANDOM_CHAR_COUNT);

		return new(tokenString, forUser, DateTime.Now + TIME_UNTIL_EXPIRE);

	}

	public DateTime ExpiryDate { get; private set; }

	public string TokenString { get; private set; }

	public User User { get; internal set; }

	public bool NeedsRefresh()
	{

		return DateTime.UtcNow < ExpiryDate - REFRESH_THRESHOLD;

	}

	public async Task RefreshToken()
	{

		ExpiryDate = DateTime.UtcNow + TIME_UNTIL_EXPIRE;

		await User.WriteNewData();

	}

	public bool IsTokenValid()
	{

		DateTime now = DateTime.UtcNow;

		return now < ExpiryDate;

	}

	public async Task RevokeToken()
	{

		await User.RevokeAccessToken(this);

	}

}
