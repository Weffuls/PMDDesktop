namespace PMDDesktop.Requests.Users;

public sealed class GetUserInfoResponse : ServerResponse
{

	public required string DisplayName { get; set; }
	public required Guid GUID { get; set; }

}
