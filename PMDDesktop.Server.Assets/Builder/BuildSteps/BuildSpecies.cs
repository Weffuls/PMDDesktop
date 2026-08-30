using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using PMDDesktop.Server.Assets.Data;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.BuildSteps;

internal static class BuildSpecies
{

	public static async Task StartBuildStep(AssetManager assets)
	{

		await BuildTopLevelSpecies(assets);

		return;

	}

	private static async Task BuildTopLevelSpecies(AssetManager assets)
	{

		using PokeApiZip zip = await ZipManager.GetPokeApiZip();
		using SpriteCollabZip spriteZip = await ZipManager.GetSpriteCollabZip();

		foreach (ZipArchiveEntry entry in zip.EnumerateSpecies())
		{

			using Stream stream = await entry.OpenAsync();

			using JsonDocument json = await JsonDocument.ParseAsync(stream);

			await BuildFullSpeciesAndVariantsAndForms(assets, json.RootElement, zip);

		}

	}

	private static async Task BuildFullSpeciesAndVariantsAndForms(AssetManager assets, JsonElement pokemonSpeciesRoot, PokeApiZip zip)
	{

		Species species = await BuildOneSpeciesAsset(pokemonSpeciesRoot);
		assets.Add(species);

		List<MetaForm> forms = [];
		List<MetaVariant> variants = [];

		// Create variants & forms and add them to the groups list.
		foreach (JsonElement formRoot in await PokeApiUtils.GetSpeciesForms(pokemonSpeciesRoot, zip))
		{

			MetaForm createdForm = await BuildOneFormAsset(species, formRoot, zip);

			forms.Add(createdForm);

		}

		foreach (MetaForm form in forms)
		{

			if (!form.IsStandaloneForm())
				continue;

			// At this point, we should only see Pokémon that fully qualify as being their own variation.
			// If we do not, adjust the code before this point.

			MetaVariant createdForm = await BuildFullVariant(species, form, forms, zip);

			variants.Add(createdForm);

		}

		// Actually add those generated variants to the asset manager.
		foreach (MetaVariant variant in variants)
		{

			// Add the variant itself. This should be unique, so crash if there's a duplicate.
			assets.Add(variant);

		}

		// Add the forms too.
		foreach (SpeciesForm form in forms)
		{

			// Make sure that, if this form already exists, it matches the data we have, so two variants can share forms.
			if (assets.TryGetAsset(form.Location, out SpeciesForm? existingForm))
				if (AssetUtils.HoldsIdenticalData(existingForm, form))
					continue;

			assets.Add(form);

		}

	}

	private static async Task<MetaVariant> BuildFullVariant(Species species, MetaForm baseForm, IEnumerable<MetaForm> potentialForms, PokeApiZip zip)
	{

		List<MetaForm> otherPotentialForms = [.. potentialForms.Where(form => baseForm != potentialForms)];

		List<MetaForm> extraForms = await LinkForms(baseForm, otherPotentialForms);

		SpeciesVariant variant = await BuildOneVariantAsset(species, baseForm, extraForms, zip);

		return new(variant);

	}

	private static async Task<List<MetaForm>> LinkForms(MetaForm baseForm, IEnumerable<MetaForm> potentialForms)
	{

		List<MetaForm> foundForms = [];

		foreach (MetaForm potentialForm in potentialForms)
		{

			// TODO: Make this work

			throw new NotImplementedException();

		}

		return foundForms;

	}


	/// <summary>
	/// Builds strictly ONE species asset. Does not build any variants or forms.
	/// </summary>
	/// <param name="pokemonSpeciesRoot">The root of a "pokemon-species" object.</param>
	/// <returns>The species asset that was built.</returns>
	/// <exception cref="InvalidDataException"></exception>
	private static async Task<Species> BuildOneSpeciesAsset(JsonElement pokemonSpeciesRoot)
	{

		int speciesNumber = pokemonSpeciesRoot.GetProperty("id").GetInt32();
		string speciesName = pokemonSpeciesRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"No name found on {pokemonSpeciesRoot}");

		AssetLocation location = new("species", $"{speciesNumber:0000}-{speciesName}");

		Species species = new(location);

		return species;

	}

	/// <summary>
	/// Builds strictly ONE variant asset. Does not build any forms.
	/// </summary>
	/// <param name="species">Species asset this Variant belongs to.</param>
	/// <param name="pokemonRoot">The root of the "pokemon" object used to create data.</param>
	/// <param name="defaultForm">SpeciesForm asset used as the default form.</param>
	/// <returns></returns>
	/// <exception cref="InvalidDataException"></exception>
	private static async Task<MetaVariant> BuildOneVariantAsset(Species species, MetaForm baseForm, IEnumerable<MetaForm> extraForms, PokeApiZip zip)
	{

		string variantName = baseForm.formRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"{baseForm.formRoot} had no name property");

		// JsonElement pokemonRoot = await PokeApiUtils.GetPokemonFromForm(pokemonFormRoot, zip);

		AssetLocation location = new(species.Location, "variants", variantName);
		SpeciesVariant variant = new(location)
		{
			Species = new(species),
			EvolutionTags = [],
			DefaultForm = new(baseForm),
			OtherForms = [.. extraForms.Select((form) => new AssetReference<SpeciesForm>(form))]
		};

		return new(variant);

	}

	/// <summary>
	/// Builds strictly ONE species form asset, and collects metadata about it.
	/// </summary>
	/// <param name="species">The species asset that this form belongs to.</param>
	/// <param name="pokemonFormRoot">The root of the "pokemon-form" object used to create data.</param>
	/// <returns>The form asset that was built, along with some metadata.</returns>
	/// <exception cref="InvalidDataException"></exception>
	private static async Task<MetaForm> BuildOneFormAsset(Species species, JsonElement pokemonFormRoot, PokeApiZip zip)
	{

		string formName = pokemonFormRoot.GetProperty("name").GetString()
			?? throw new InvalidDataException($"{pokemonFormRoot} had no name property");

		JsonElement pokemonRoot = await PokeApiUtils.GetPokemonFromForm(pokemonFormRoot, zip);

		AssetLocation location = new(species.Location, "forms", formName);
		SpeciesForm form = new(location)
		{
			Types = PokeApiUtils.CreateFormTypeReferences(pokemonFormRoot),
			Stats = PokeApiUtils.CreatePokemonBattleStats(pokemonRoot)
		};

		return new()
		{
			form = form,
			formRoot = pokemonFormRoot,
			originalFormName = formName
		};

	}

}
