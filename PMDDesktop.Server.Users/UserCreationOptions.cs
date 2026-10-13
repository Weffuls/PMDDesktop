namespace PMDDesktop.Server.Users;

public sealed record class UserCreationOptions
{

	/// <summary>
	/// The initial value to set <see cref="User.Name"/>.
	/// </summary>
	public required string DisplayName { get; set; }

	/// <summary>
	/// The value to hash to create <see cref="User.HashedPassword"/>.
	/// </summary>
	public string? PlainTextPassword { get; set; }

	/// <summary>
	/// The initial value to set <see cref="User.LoginHandle"/>. Must be unique.
	/// </summary>
	public string? LoginHandle { get; set; }

	public bool IsAdmin { get; set; } = false;

}
