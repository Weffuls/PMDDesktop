namespace PMDDesktop.Requests.Users;

public sealed class UserInfoResponse : ServerJsonResponse
{

	public required string DisplayName { get; set; }
	public required Guid GUID { get; set; }
	public DataID? LoginHandle { get; set; }
	public bool? HasPassword { get; set; }

}
