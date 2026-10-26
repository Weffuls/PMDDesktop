using PMDDesktop.Requests.Permissions;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;
using PMDDesktop.Server.Users.Roles;

namespace PMDDesktop.Server.RequestHandlers.Permissions;

public class ListPermissionsRequestHandler : JsonRequestHandler<ListPermissionsRequest, ListPermissionsResponse>
{

	protected override async Task<ResponseJsonResult<ListPermissionsResponse, ResponseResultError>> CreateResponse(ListPermissionsRequest request, GameServer game, User? requestingUser)
	{

		return new(new ListPermissionsResponse()
		{
			Permissions = [.. UserPermission.ALL.Select(GenericResponseCreators.CreatePermissionInfoResponse)]
		});

	}

}
