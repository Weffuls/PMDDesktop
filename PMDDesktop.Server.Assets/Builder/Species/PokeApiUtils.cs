using PMDDesktop.GameData;
using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.Collections.Immutable;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// <para>This file contains extremely specialized helper functions for <see cref="BuildSpecies"/> relating to <see href="https://github.com/PokeAPI/api-data">PokeAPI's api-data</see>.</para>
/// <para>They are delegated here to help <see cref="BuildSpecies"/> be easy to follow the flow of.</para>
/// </summary>
internal static class PokeApiUtils
{

	/// <summary>
	/// <para>Takes in an <paramref name="apiUrl"/> and returns a <see cref="JsonElement"/> using the <see cref="PokeApiZip"/> that said <paramref name="apiUrl"/> would usually be pointing to.</para>
	/// </summary>
	/// <param name="apiUrl">URL found inside an API file. This is the string literal found in the json data of the API.</param>
	/// <param name="zip">Zip file to use to open the API URL</param>
	/// <returns></returns>
	internal static async Task<JsonElement> ResolveApiUrl(string apiUrl, PokeApiZip zip)
	{

		ZipArchiveEntry entry = zip.GetEntryFromApiUrl(apiUrl);
		Stream stream = await entry.OpenAsync();

		return (await JsonDocument.ParseAsync(stream)).RootElement.Clone();

	}

	/// <summary>
	/// <para>Enumerates all "pokemon" in a "pokemon-species" object.</para>
	/// </summary>
	/// <param name="pokemonSpeciesRoot">The root of a "pokemon-species" json from the API.</param>
	/// <param name="zip">The <see cref="PokeApiZip"/> to use to retrieve "pokemon" <see cref="JsonElement"/>s.</param>
	/// <returns>Enumerable with all "pokemon" roots in that "pokemon-species" object as <see cref="JsonElement"/>s.</returns>
	internal static async Task<IOrderedEnumerable<JsonElement>> GetSpeciesPokemonVarieties(JsonElement pokemonSpeciesRoot, PokeApiZip zip)
	{

		List<JsonElement> list = [];
		JsonElement array = pokemonSpeciesRoot.GetProperty("varieties");

		foreach (JsonElement variety in array.EnumerateArray())
		{

			string apiUrl = variety.GetProperty("pokemon").GetProperty("url").GetString()
				?? throw new InvalidDataException($"Unable to find Pokemon URL in {variety} in {pokemonSpeciesRoot}");

			list.Add(await ResolveApiUrl(apiUrl, zip));

		}

		return list.OrderBy((a) => a.GetProperty("id").GetInt32());

	}

	/// <summary>
	/// <para>Enumerates all "pokemon-forms" in a "pokemon-species" object.</para>
	/// </summary>
	/// <param name="pokemonSpeciesRoot">The root of a "pokemon-species" json from the API.</param>
	/// <param name="zip">The <see cref="PokeApiZip"/> to use to retrieve "pokemon-forms" and "pokemon" <see cref="JsonElement"/>s.</param>
	/// <returns>Enumerable with all "pokemon-forms" roots in that "pokemon-species" object as <see cref="JsonElement"/>s.</returns>
	internal static async Task<IOrderedEnumerable<JsonElement>> GetSpeciesForms(JsonElement pokemonSpeciesRoot, PokeApiZip zip)
	{

		List<JsonElement> formRoots = [];

		foreach (JsonElement pokemonRoot in await GetSpeciesPokemonVarieties(pokemonSpeciesRoot, zip))
			foreach (JsonElement formRoot in await GetPokemonForms(pokemonRoot, zip))
				if (!formRoots.Any((form) => formRoot.GetProperty("id").GetInt32() == form.GetProperty("id").GetInt32())) // Guard against duplicates, shouldn't happen though.
					formRoots.Add(formRoot);

		return formRoots.OrderBy((a) => a.GetProperty("id").GetInt32());

	}

	/// <summary>
	/// <para>Enumerates all "pokemon-forms" in a "pokemon" object.</para>
	/// </summary>
	/// <param name="pokemonRoot">The root of a "pokemon" json from the API.</param>
	/// <param name="zip">The <see cref="PokeApiZip"/> to use to retrieve "pokemon-forms" <see cref="JsonElement"/>s.</param>
	/// <returns>Enumerable with all "pokemon-forms" roots in that "pokemon" object as <see cref="JsonElement"/>s.</returns>
	internal static async Task<IOrderedEnumerable<JsonElement>> GetPokemonForms(JsonElement pokemonRoot, PokeApiZip zip)
	{

		List<JsonElement> list = [];
		JsonElement array = pokemonRoot.GetProperty("forms");

		foreach (JsonElement form in array.EnumerateArray())
		{

			string apiUrl = form.GetProperty("url").GetString()
				?? throw new InvalidDataException($"Unable to find Pokemon URL in {form} in {pokemonRoot}");

			list.Add(await ResolveApiUrl(apiUrl, zip));

		}

		return list.OrderBy((a) => a.GetProperty("id").GetInt32());

	}

	/// <summary>
	/// <para>Find the "pokemon" that's the 'parent' of the provided "pokemon-form"</para>
	/// </summary>
	/// <param name="pokemonFormRoot">The root of a "pokemon-form" json from the API.</param>
	/// <param name="zip">The <see cref="PokeApiZip"/> to use to retrieve the "pokemon" <see cref="JsonElement"/>.</param>
	/// <returns>The root of the "pokemon" json object that's the 'parent' of the provided <paramref name="pokemonFormRoot"/> as a <see cref="JsonElement"/>.</returns>
	internal static async Task<JsonElement> GetPokemonFromForm(JsonElement pokemonFormRoot, PokeApiZip zip)
	{

		string apiUrl = pokemonFormRoot.GetProperty("pokemon").GetProperty("url").GetString()
			?? throw new InvalidDataException($"No \"pokemon\" -> \"url\" string found on {pokemonFormRoot}");

		return await ResolveApiUrl(apiUrl, zip);

	}

	/// <summary>
	/// <para>Find the "pokemon-species" that's the 'parent' of the provided "pokemon"</para>
	/// </summary>
	/// <param name="pokemonRoot">The root of a "pokemon" json from the API.</param>
	/// <param name="zip">The <see cref="PokeApiZip"/> to use to retrieve the "pokemon-species" <see cref="JsonElement"/>.</param>
	/// <returns>The root of the "pokemon-species" json object that's the 'parent' of the provided <paramref name="pokemonRoot"/> as a <see cref="JsonElement"/>.</returns>
	internal static async Task<JsonElement> GetSpeciesFromPokemon(JsonElement pokemonRoot, PokeApiZip zip)
	{

		string apiUrl = pokemonRoot.GetProperty("species").GetProperty("url").GetString()
			?? throw new InvalidDataException($"No \"species\" -> \"url\" string found on {pokemonRoot}");

		return await ResolveApiUrl(apiUrl, zip);

	}

	/// <summary>
	/// <para>Find the "pokemon-species" that's the 'parent' of the provided "pokemon-form"</para>
	/// </summary>
	/// <param name="pokemonFormRoot">The root of a "pokemon-form" json from the API.</param>
	/// <param name="zip">The <see cref="PokeApiZip"/> to use to retrieve the "pokemon-species" and "pokemon" <see cref="JsonElement"/>s.</param>
	/// <returns>The root of the "pokemon-species" json object that's the 'parent' of the provided <paramref name="pokemonFormRoot"/> as a <see cref="JsonElement"/>.</returns>
	internal static async Task<JsonElement> GetSpeciesFromForm(JsonElement pokemonFormRoot, PokeApiZip zip)
	{

		return await GetSpeciesFromPokemon(await GetPokemonFromForm(pokemonFormRoot, zip), zip);

	}

	/// <summary>
	/// <para>Opinionated function to determine if this "pokemon-form" api object makes sense to be the root of a variant.</para>
	/// <para>e.g. is it not a mega, or a form that's conditional?</para>
	/// </summary>
	/// <param name="pokemonFormRoot">The root of the "pokemon-form" to be evaluated.</param>
	/// <returns>true if the "pokemon-form" makes sense as a standalone form, false otherwise.</returns>
	internal static bool IsPokemonFormStandalone(JsonElement pokemonFormRoot)
	{

		JsonElement triggerConditions = pokemonFormRoot.GetProperty("trigger_conditions");

		return triggerConditions.GetArrayLength() == 0;

	}

	/// <summary>
	/// Check if the provided <paramref name="pokemonFormRoot"/> references any <paramref name="baseFormNames"/> as a "base_form".
	/// </summary>
	/// <param name="pokemonFormRoot">A <see cref="JsonElement"/> of the root of a "pokemon-form".</param>
	/// <param name="baseFormNames">The names of "base_form" you'd like to match.</param>
	/// <param name="returnTrueIfNull">If there are no listed base forms on <paramref name="pokemonFormRoot"/>, should this function return true?</param>
	/// <returns></returns>
	/// <exception cref="InvalidDataException">Throws if it cannot find properties in the <paramref name="pokemonFormRoot"/></exception>
	internal static bool IsPokemonFormWithMatchingBaseForm(JsonElement pokemonFormRoot, IEnumerable<string> baseFormNames, bool returnTrueIfNull)
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

		// This is, if properBaseForms is 0, return returnTrueIfNull. Otherwise, false.
		return properBaseForms == 0 && returnTrueIfNull;

	}

	/// <summary>
	/// Takes in a "pokemon-form" to create references to the default location of their <see cref="PokemonType"/> assets.
	/// </summary>
	/// <param name="formRootElement">A "pokemon-form" object"</param>
	/// <returns>References to the default location of those types.</returns>
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
	/// Takes in a "pokemon" to create <see cref="BattleStats"/> based on the input.
	/// </summary>
	/// <param name="pokemonRootElement">A "pokemon" object"</param>
	/// <returns><see cref="BattleStats"/> from the "pokemon" object.</returns>
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

}
