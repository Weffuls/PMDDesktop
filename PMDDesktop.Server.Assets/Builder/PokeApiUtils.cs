using PMDDesktop.GameData;
using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// This file contains extremely specialized helper functions for BuildSpecies relating to PokeAPI's api-data.
/// They are delegated here to help BuildSpecies be easy to follow the flow of.
/// </summary>
internal static class PokeApiUtils
{

	/// <summary>
	/// Takes in an Api Url and returns a JsonElement using the zip that said Url would usually be pointing to.
	/// </summary>
	/// <param name="apiUrl">URL found inside an API file.</param>
	/// <param name="zip">Zip file to use to open the API URL</param>
	/// <returns></returns>
	internal static async Task<JsonElement> ResolveApiUrl(string apiUrl, PokeApiZip zip)
	{

		ZipArchiveEntry entry = zip.GetEntryFromApiUrl(apiUrl);
		Stream stream = await entry.OpenAsync();

		return (await JsonDocument.ParseAsync(stream)).RootElement.Clone();

	}

	/// <summary>
	/// Enumerates all "pokemon" in a "pokemon-species" object.
	/// </summary>
	/// <param name="element"></param>
	/// <returns>All "pokemon" in that "pokemon-species" object</returns>
	/// <exception cref="InvalidDataException"></exception>
	internal static async Task<IOrderedEnumerable<JsonElement>> GetSpeciesPokemonVarieties(JsonElement element, PokeApiZip zip)
	{

		List<JsonElement> list = [];
		JsonElement array = element.GetProperty("varieties");

		foreach (JsonElement variety in array.EnumerateArray())
		{

			string apiUrl = variety.GetProperty("pokemon").GetProperty("url").GetString()
				?? throw new InvalidDataException($"Unable to find Pokemon URL in {variety} in {element}");

			list.Add(await ResolveApiUrl(apiUrl, zip));

		}

		return list.OrderBy((a) => a.GetProperty("id").GetInt32());

	}

	/// <summary>
	/// Enumerates all "pokemon-forms" in a "pokemon-species" object.
	/// </summary>
	/// <param name="speciesRoot"></param>
	/// <returns>All "pokemon" in that "pokemon-species" object</returns>
	/// <exception cref="InvalidDataException"></exception>
	internal static async Task<IOrderedEnumerable<JsonElement>> GetSpeciesForms(JsonElement speciesRoot, PokeApiZip zip)
	{

		List<JsonElement> formRoots = [];

		foreach (JsonElement pokemonRoot in await GetSpeciesPokemonVarieties(speciesRoot, zip))
			foreach (JsonElement formRoot in await GetPokemonForms(pokemonRoot, zip))
				if (!formRoots.Any((form) => formRoot.GetProperty("id").GetInt32() == form.GetProperty("id").GetInt32())) // Guard against duplicates, shouldn't happen though.
					formRoots.Add(formRoot);

		return formRoots.OrderBy((a) => a.GetProperty("id").GetInt32());

	}

	/// <summary>
	/// Enumerates all "pokemon-forms" in a "pokemon" object.
	/// </summary>
	/// <param name="element">The root element in a "pokemon" object.</param>
	/// <returns></returns>
	/// <exception cref="InvalidDataException"></exception>
	internal static async Task<IOrderedEnumerable<JsonElement>> GetPokemonForms(JsonElement element, PokeApiZip zip)
	{

		List<JsonElement> list = [];
		JsonElement array = element.GetProperty("forms");

		foreach (JsonElement form in array.EnumerateArray())
		{

			string apiUrl = form.GetProperty("url").GetString()
				?? throw new InvalidDataException($"Unable to find Pokemon URL in {form} in {element}");

			list.Add(await ResolveApiUrl(apiUrl, zip));

		}

		return list.OrderBy((a) => a.GetProperty("id").GetInt32());

	}

	internal static async Task<JsonElement> GetPokemonFromForm(JsonElement pokemonFormRoot, PokeApiZip zip)
	{

		string apiUrl = pokemonFormRoot.GetProperty("pokemon").GetProperty("url").GetString()
			?? throw new InvalidDataException($"No \"pokemon\" -> \"url\" string found on {pokemonFormRoot}");

		return await ResolveApiUrl(apiUrl, zip);

	}

	internal static async Task<JsonElement> GetSpeciesFromPokemon(JsonElement pokemonRoot, PokeApiZip zip)
	{

		string apiUrl = pokemonRoot.GetProperty("species").GetProperty("url").GetString()
			?? throw new InvalidDataException($"No \"species\" -> \"url\" string found on {pokemonRoot}");

		return await ResolveApiUrl(apiUrl, zip);

	}

	internal static async Task<JsonElement> GetSpeciesFromForm(JsonElement pokemonFormRoot, PokeApiZip zip)
	{

		return await GetSpeciesFromPokemon(await GetPokemonFromForm(pokemonFormRoot, zip), zip);

	}

	/// <summary>
	/// <para>Does this "pokemon-form" api object make sense to be the root of a variant?</para>
	/// <para>e.g. is it not a mega, or a form that's conditional?</para>
	/// </summary>
	/// <param name="pokemonRoot"></param>
	/// <returns></returns>
	internal static bool IsPokemonFormStandalone(JsonElement pokemonFormRoot)
	{

		JsonElement triggerConditions = pokemonFormRoot.GetProperty("trigger_conditions");

		return triggerConditions.GetArrayLength() == 0;

	}

	internal static bool IsPokemonFormWithoutReferencedBaseForms(JsonElement pokemonFormRoot)
	{

		return IsPokemonFormWithMatchingBaseForm(pokemonFormRoot, [], true);

	}

	internal static bool IsPokemonFormWithMatchingBaseForm(JsonElement pokemonFormRoot, IEnumerable<string> baseFormNames, bool returnIfNull)
	{

		JsonElement triggerConditions = pokemonFormRoot.GetProperty("trigger_conditions");

		// Not every condition has a base_form. So we can't just check that the array length is 0.
		int properBaseForms = 0;

		foreach (JsonElement condition in triggerConditions.EnumerateArray())
		{

			// Not every condition has a base_form. Continue if it doesn't.
			if (!condition.TryGetProperty("base_form", out JsonElement baseForm))
				continue;

			string foundBaseFormName = baseForm.GetProperty("name").GetString()
				?? throw new InvalidDataException($"Read string \"name\" from {baseForm} was null.");

			if (baseFormNames.Contains(foundBaseFormName))
				return true;

			++properBaseForms;

		}

		// This is, if properBaseForms is 0, return returnIfNull. Otherwise, false.
		return properBaseForms == 0 && returnIfNull;

	}

	/// <summary>
	/// Takes in a "pokemon-form" to create stats.
	/// </summary>
	/// <param name="formRootElement">A "pokemon-form" object"</param>
	/// <returns>Type references to the default location of those types.</returns>
	/// <exception cref="InvalidDataException"></exception>
	internal static ImmutableArray<AssetReference<PokemonType>> CreateFormTypeReferences(JsonElement formRootElement)
	{

		JsonElement formTypesElement = formRootElement.GetProperty("types");

		ImmutableArray<AssetReference<PokemonType>>.Builder list = ImmutableArray.CreateBuilder<AssetReference<PokemonType>>(formTypesElement.GetArrayLength());

		foreach (JsonElement type in formTypesElement.EnumerateArray())
		{

			string typeName = type.GetProperty("type").GetProperty("name").GetString()
				?? throw new InvalidDataException($"Read string \"name\" from {type} was null.");

			list.Add(new(PokemonType.DefaultTypeLocation(typeName)));

		}

		return list.ToImmutable();

	}

	/// <summary>
	/// Takes in a "pokemon" to create stats.
	/// </summary>
	/// <param name="pokemonRootElement">A "pokemon" object"</param>
	/// <returns>BattleStats from the "pokemon" object.</returns>
	/// <exception cref="InvalidDataException"></exception>
	internal static BattleStats CreatePokemonBattleStats(JsonElement pokemonRootElement)
	{

		JsonElement statsElement = pokemonRootElement.GetProperty("stats");

		BattleStats stats = new();

		foreach (JsonElement stat in statsElement.EnumerateArray())
		{

			string typeName = stat.GetProperty("stat").GetProperty("name").GetString()?.ToLower()
				?? throw new InvalidDataException($"Read string \"stat\" -> \"name\" from {stat} was null.");

			int result = stat.GetProperty("base_stat").GetInt32();

			if (typeName == "hp")
				stats.Hp = result;
			else if (typeName == "attack")
				stats.PhysicalAttack = result;
			else if (typeName == "defense")
				stats.PhysicalDefense = result;
			else if (typeName == "special-attack")
				stats.SpecialAttack = result;
			else if (typeName == "special-defense")
				stats.SpecialDefense = result;
			else if (typeName == "speed")
				stats.Speed = result;
			else
				throw new InvalidDataException($"{typeName} didn't match a hardcoded type name.");

		}

		return stats;

	}

	internal static bool IsPartInNameString(string nameString, string part)
	{

		return nameString.Split('-').Contains(part);

	}

}
