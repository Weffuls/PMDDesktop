using PMDDesktop.Requests;
using PMDDesktop.Server.Assets.Data;
using PMDDesktop.Server.Game;

namespace PMDDesktop.Server.RequestHandlers;

public class ListSpeciesRequestHandler : JsonRequestHandler<ListSpeciesRequest, ListSpeciesResponse>
{

	protected override ListSpeciesResponse CreateResponse(ListSpeciesRequest request, GameServer game)
	{

		return new()
		{
			SpeciesIDs = [.. game.State.Assets.OfType<Species>().Select((species) => (string)species.Location)]
		};

	}

}
