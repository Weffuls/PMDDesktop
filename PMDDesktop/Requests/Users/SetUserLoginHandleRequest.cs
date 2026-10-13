namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/set-handle")]
public sealed class SetUserLoginHandleRequest : ServerRequest<UserInfoResponse>
{

	/// <summary>
	/// The <see cref="Guid"/> of the user you're trying to set the login handle of.
	/// </summary>
	public required Guid GUID { get; set; }

	/// <summary>
	/// <para>The login handle for the account.</para>
	/// <para>Can be null to unset the handle (to prevent logins with handle/password).</para>
	/// <para>TEMP: handles must be lowercase, 0-9, or hyphen (-).</para>
	/// </summary>
	public required string? NewLoginHandle { get; set; }

}
