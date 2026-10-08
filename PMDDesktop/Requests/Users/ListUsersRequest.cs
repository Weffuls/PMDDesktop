namespace PMDDesktop.Requests.Users;

[ServerRequest("/api/users/list")]
public sealed class ListUsersRequest : ServerRequest<ListUsersResponse>
{

}
