namespace PMDDesktop.Requests.Users;

public sealed class ListUsersResponse : ServerResponse
{

	/// <summary>
	/// All currently existing users, presented in an arbitrary order.
	/// </summary>
	public required UserInfoResponse[] Users { get; set; }

}
