namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/create")]
public sealed class CreateUserRequest : ServerRequest<UserInfoResponse>
{

	public required string NewName { get; set; }

	public required string? NewLoginHandle { get; set; }

	public required string? NewPassword { get; set; }

}
