namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/set-name")]
public sealed class SetUserNameRequest : ServerRequest<UserInfoResponse>
{

	public required Guid GUID { get; set; }

	public required string NewName { get; set; }

}
