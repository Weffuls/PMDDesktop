using System.Security.Cryptography;

namespace PMDDesktop.Server.Users;

public sealed class AccessToken
{

	public static readonly TimeSpan TIME_UNTIL_EXPIRE = new(8, 0, 0, 0); // 8 days.
	public static readonly TimeSpan REFRESH_THRESHOLD = new(23, 0, 0); // Ideally if you log in once a week, you'll stay logged in.

	public static readonly int RANDOM_BYTE_COUNT = 32;

	internal AccessToken(string token, User forUser, DateTime expires)
	{

		TokenString = token;
		User = new(forUser);
		ExpiryDate = expires;

	}

	internal static AccessToken CreateNewToken(User forUser)
	{
		
		string tokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(RANDOM_BYTE_COUNT));

		return new(tokenString, forUser, DateTime.Now + TIME_UNTIL_EXPIRE);

	}

	public DateTime ExpiryDate {get; private set;}

	public string TokenString {get; private set;}

	public UserReference User {get; internal set;}

	public void RefreshToken()
	{

		ExpiryDate = DateTime.UtcNow + TIME_UNTIL_EXPIRE;

	}

	public bool IsTokenValid()
	{

		DateTime now = DateTime.UtcNow;

		return now < ExpiryDate;

	}

}