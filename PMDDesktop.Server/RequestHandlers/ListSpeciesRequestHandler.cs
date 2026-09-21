using PMDDesktop.Requests;
using PMDDesktop.Server.Assets.Data;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers;

public class ListSpeciesRequestHandler : JsonRequestHandler<ListSpeciesRequest, ListSpeciesResponse>
{

	protected override async Task<ListSpeciesResponse> CreateResponse(ListSpeciesRequest request, GameServer game, User? requestingUser)
	{

		return new()
		{
			SpeciesIDs = [.. game.State.Assets.OfType<Species>().Select((species) => (string)species.Location)]
		};

	}

}
