namespace PMDDesktop.Requests.Roles;

[ServerRequest("/api/roles/get-info")]
public sealed class GetRoleInfoRequest : ServerRequest<RoleInfoResponse>
{

	[QueryParameter]
	public required Guid GUID { get; set; }

}
