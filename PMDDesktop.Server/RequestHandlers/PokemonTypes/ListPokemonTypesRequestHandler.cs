using PMDDesktop.Requests.PokemonTypes;
using PMDDesktop.Server.Assets.Data;
using PMDDesktop.Server.Game;
using PMDDesktop.Server.Users;

namespace PMDDesktop.Server.RequestHandlers.PokemonTypes;

public class ListPokemonTypesRequestHandler : JsonRequestHandler<ListPokemonTypesRequest, ListPokemonTypesResponse>
{

	protected override async Task<ListPokemonTypesResponse> CreateResponse(ListPokemonTypesRequest request, GameServer game, User? requestingUser)
	{

		return new()
		{
			Types = [.. game.State.Assets.OfType<PokemonType>().Select(TypeAssetToTypeEntry)]
		};

	}

	private static ListPokemonTypesResponse.TypeEntry TypeAssetToTypeEntry(PokemonType type)
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
