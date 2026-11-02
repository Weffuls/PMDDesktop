namespace PMDDesktop.Requests.Users;

public sealed class UserInfoResponse : ServerJsonResponse
{

	public required string DisplayName { get; set; }
	public required Guid GUID { get; set; }
	public string? LoginHandle { get; set; }
	public bool? HasPassword { get; set; }

}
