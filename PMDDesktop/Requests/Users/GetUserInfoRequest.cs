namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/get-info")]
public sealed class GetUserInfoRequest : ServerRequest<UserInfoResponse>
{

	[QueryParameter]
	public required Guid GUID { get; set; }

}
