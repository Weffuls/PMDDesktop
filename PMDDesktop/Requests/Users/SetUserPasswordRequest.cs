namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/set-password")]
public sealed class SetUserPasswordRequest : ServerRequest<UserInfoResponse>
{

	/// <summary>
	/// The <see cref="Guid"/> of the user you're trying to set the password of.
	/// </summary>
	public required Guid GUID { get; set; }

	/// <summary>
	/// <para>The new password for the account (plain text; hashed server-side).</para>
	/// <para>Can be null to unset the password (to prevent logins with handle/password).</para>
	/// </summary>
	public required string? NewPassword { get; set; }

}
