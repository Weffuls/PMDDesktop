using PMDDesktop.Requests;
using PMDDesktop.Server.Assets.Data;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers;

public class ListTypesRequestHandler : JsonRequestHandler<ListTypesRequest, ListTypesResponse>
{

	protected override async Task<ListTypesResponse> CreateResponse(ListTypesRequest request, GameServer game, User? requestingUser)
	{

		return new()
		{
			Types = [.. game.State.Assets.OfType<PokemonType>().Select(TypeAssetToTypeEntry)]
		};

	}

	private static ListTypesResponse.TypeEntry TypeAssetToTypeEntry(PokemonType type)
	{

		return new()
		{
			ID = type.Location,
			Immunities = [.. type.Immunities.Select((reference) => reference.Location.ToString())],
			Weaknesses = [.. type.Weaknesses.Select((reference) => reference.Location.ToString())],
			Resistances = [.. type.Resistances.Select((reference) => reference.Location.ToString())]
		};

	}

}
