using PMDDesktop.Requests.Species;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.Species;

public class ListSpeciesRequestHandler : JsonRequestHandler<ListSpeciesRequest, ListSpeciesResponse>
{

	protected override async Task<ResponseResult<ListSpeciesResponse, ResponseResultError>> CreateResponse(ListSpeciesRequest request, GameServer game, User? requestingUser)
	{

		return new(new ListSpeciesResponse()
		{
			SpeciesIDs = [.. game.State.Assets.OfType<Assets.Data.Species>().Select((species) => (string)species.Location)]
		});

	}

}
