using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace PMDDesktop.Server.Users;

public sealed class User
{

	static PasswordHasher<User> PASSWORD_HASHER = new();

	[JsonConstructor]
	internal User()
	{

	}

	/// <summary>
	/// <para>A string representing a <see cref="User"/>'s name.</para>
	/// </summary>
	[JsonInclude]
	public string Name {get; internal set;} = string.Empty;

	/// <summary>
	/// <para>A string to use for logging in. Should only ever contain [a-z], [0-9] and (-)</para>
	/// <para>Does not neccessarily match <see cref="Name"/> in most senarios.</para>
	/// <para>Should be unique.</para>
	/// </summary>
	[JsonInclude]
	public string LoginHandle {get; internal set;} = string.Empty;

	/// <summary>
	/// <para>Hashed password used to log in.</para>
	/// <para>In the event that it is null, access to this account cannot be granted via password.</para>
	/// </summary>
	[JsonInclude]
	internal string? HashedPassword {get; set;}

	/// <summary>
	/// <para>A unique <see cref="Guid"/> for each <see cref="User"/>. Stays consistant even when the <see cref="User"/>'s handle changes.</para>
	/// <para>This should be used for long-term references to a specific <see cref="User"/>.</para>
	/// </summary>
	[JsonIgnore]
	public Guid GUID {get; internal set;} = Guid.NewGuid();

	/// <summary>
	/// Salt and hash the user's password, then store it in HashedPassword.
	/// </summary>
	/// <param name="plainText"></param>
	/// <returns></returns>
	public async Task SetPassword(string plainText)
	{

		HashedPassword = PASSWORD_HASHER.HashPassword(this, plainText);

	}

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

		return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;

	}

	internal string GetUserFolder()
	{

		return Path.Combine(AppContext.BaseDirectory, "users", GUID.ToString());

	}

	public string GetUserJSON()
	{

		return Path.Combine(GetUserFolder(), "user.json");

	}

	public async Task WriteNewData()
	{

		

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

}
