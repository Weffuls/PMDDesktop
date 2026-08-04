using PMDDesktop.Requests;
using PMDDesktop.Server.Assets.Data;
using PMDDesktop.Server.Game;

namespace PMDDesktop.Server.RequestHandlers;

public class ListTypesRequestHandler : JsonRequestHandler<ListTypesRequest, ListTypesResponse>
{

	protected override ListTypesResponse CreateResponse(ListTypesRequest request, GameServer game)
	{

		return new()
		{
			TypeIDs = [.. game.State.Assets.OfType<PokemonType>().Select((type) => (string)type.Location)]
		};

	}

}
