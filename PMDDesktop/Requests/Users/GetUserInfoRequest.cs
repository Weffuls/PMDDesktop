namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/get-info")]
public sealed class GetUserInfoRequest : ServerRequest<GetUserInfoResponse>
{

	[QueryParameter]
	public required Guid GUID { get; set; }

}
