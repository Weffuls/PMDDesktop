using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// <para>Static functions to help build assets of the following types:</para> 
/// <list type="bullet">
/// <item><see cref="Data.Species"/></item>
/// <item><see cref="Data.SpeciesForm"/></item>
/// <item><see cref="Data.SpeciesVariant"/></item>
/// <item><see cref="Data.SpeciesPortraits"/></item>
/// <item><see cref="Data.SpeciesSprites"/></item>
/// </list>
/// <para>These are a full routine that will fill an <see cref="AssetManager"/>  with the results.</para>
/// </summary>
internal static class BuildSpecies
{

	/// <summary>
	/// <para>Run the full <see cref="BuildSpecies"/> routine, using zips downloaded from the internet, and adding the results to the provided <see cref="AssetManager"/>.</para>
	/// </summary>
	/// <param name="assetManager">The <see cref="AssetManager"/> that the built assets will be added to.</param>
	/// <returns>Completes Task on completion; results are stored in <paramref name="assetManager"/>.</returns>
	/// <remarks>
	/// <para>Uses functions from <see cref="ZipManager"/> to find the required zip files; they will be downloaded from the internet if they are unable to be located or read from the filesystem.</para>
	/// <para>Unfit for unit testing, since it will read/write to the file system during the process of obtaining those zips. Consider using <see cref="BuildWithCustomZips"/> instead.</para>
	/// </remarks>
	public static async Task StartBuildStep(AssetManager assetManager)
	{

		using PokeApiZip apiZip = await ZipManager.GetPokeApiZip();
		using SpriteCollabZip spriteZip = await ZipManager.GetSpriteCollabZip();

		await BuildAllSpecies(assetManager, apiZip, spriteZip);

		return;

	}

	/// <summary>
	/// <para>Run the full <see cref="BuildSpecies"/> routine, using the provided <see cref="PokeApiZip"/> and <see cref="SpriteCollabZip"/>, and adding the results to the provided <see cref="AssetManager"/>.</para>
	/// </summary>
	/// <param name="assetManager">The <see cref="AssetManager"/> that the built assets will be added to.</param>
	/// <param name="apiZip">The <see cref="PokeApiZip"/> to be used to pull species data from.</param>
	/// <param name="spriteZip">The <see cref="SpriteCollabZip"/> to be used to pull sprite data from.</param>
	/// <returns>Completes Task on completion; results are stored in <paramref name="assetManager"/>.</returns>
	/// <remarks>
	/// <para>The routine, run with this entry point, has no side effects. It is safe to use in unit testing.</para>
	/// <para>If you are not unit testing, consider looking at <see cref="StartBuildStep"/>.</para> 
	/// </remarks>
	public static async Task BuildWithCustomZips(AssetManager assetManager, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		await BuildAllSpecies(assetManager, apiZip, spriteZip);

		return;

	}

	/// <summary>
	/// <para>Iterates through all "pokemon-species" found in <paramref name="apiZip"/>, and builds all assets needed for those species.</para>
	/// </summary>
	/// <param name="assetManager">The <see cref="AssetManager"/> the built assets will be added to.</param>
	/// <param name="apiZip">The <see cref="PokeApiZip"/> to be used to pull species data from.</param>
	/// <param name="spriteZip">The <see cref="SpriteCollabZip"/> to be used to pull sprite data from.</param>
	/// <returns>Completes Task once iteration and building is finished; results are stored in <paramref name="assetManager"/>.</returns>
	private static async Task BuildAllSpecies(AssetManager assetManager, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		foreach (ZipArchiveEntry entry in apiZip.EnumerateSpecies())
		{

			using Stream stream = await entry.OpenAsync();

			using JsonDocument json = await JsonDocument.ParseAsync(stream);

			await BuildFullSpecies(assetManager, json.RootElement, apiZip, spriteZip);

		}

	}

	/// <summary>
	/// <para>Build all the assets needed for the <paramref name="pokemonSpeciesRoot"/> provided, then add them to <paramref name="assetManager"/></para>
	/// </summary>
	/// <param name="assetManager">The <see cref="AssetManager"/> the built assets will be added to.</param>
	/// <param name="pokemonSpeciesRoot"><see cref="JsonElement"/> containing the root element of a "pokemon-species" to be built.</param>
	/// <param name="apiZip">The <see cref="PokeApiZip"/> to be used to pull species data from.</param>
	/// <param name="spriteZip">The <see cref="SpriteCollabZip"/> to be used to pull sprite data from.</param>
	/// <returns>Completes Task once this species' assets are built and added to <paramref name="assetManager"/>.</returns>
	private static async Task BuildFullSpecies(AssetManager assetManager, JsonElement pokemonSpeciesRoot, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		MetaSpecies species = new(pokemonSpeciesRoot, apiZip, spriteZip);

		species.metaAssets.AddRange(await SpriteCollabUtils.GetAllVisuals(species));

		// Create variants & forms and add them to the groups list.
		foreach (JsonElement formRoot in await PokeApiUtils.GetSpeciesForms(pokemonSpeciesRoot, apiZip))
		{

			MetaForm createdForm = new(species, formRoot);

			bool needsGenderSplit = false;
			if (createdForm.genderAlignment == GenderAlignment.None)
			{

				IEnumerable<MetaVisual> visuals = await GetClosestVisualMatches(createdForm, species);

				bool seenMale = false;
				bool seenFemale = false;

				foreach (MetaVisual visual in visuals)
				{

					if (visual.genderAlignment == GenderAlignment.Male)
						seenMale = true;

					if (visual.genderAlignment == GenderAlignment.Female)
						seenFemale = true;

				}

				if (seenMale && seenFemale)
					needsGenderSplit = true;

			}

			if (needsGenderSplit)
			{

				// Create female split.
				MetaForm femaleForm = new(createdForm)
				{
					genderAlignment = GenderAlignment.Female,
					Name = createdForm.Name + "-female"
				};
				species.metaAssets.Add(femaleForm);

				// Adjust to create male split.
				createdForm.genderAlignment = GenderAlignment.Male;
				createdForm.Name += "-male";

			}

			species.metaAssets.Add(createdForm);

		}

		// Assign visuals to forms.
		foreach (MetaForm form in species.metaAssets.OfType<MetaForm>())
		{
			foreach (MetaVisual visual in await GetClosestVisualMatches(form, species))
			{

				visual.ForForms.Add(form);

			}
		}

		// Try to merge forms.
		// Yes this loop is slow.
		while (true)
		{

			bool foundOne = false;

			foreach (MetaForm merger in species.metaAssets.OfType<MetaForm>())
			{

				foreach (MetaForm target in species.metaAssets.OfType<MetaForm>())
					if (merger.CouldFormsBeMerged(target))
					{

						merger.MergeForm(target);

						foundOne = true;
						break;

					}

				if (foundOne) break;
			}
			if (!foundOne) break;
		}

		// Create variants from forms.
		foreach (MetaForm form in species.metaAssets.OfType<MetaForm>().ToArray()) // ToArray() is used to keep the enumerator valid while editing species.metaAssets
		{

			if (!form.IsStandaloneForm())
				continue;

			// At this point, we should only see Pokémon that fully qualify as being their own variation.
			// If we do not, adjust the code before this point.

			species.metaAssets.AddRange(await BuildGenderedVariants(species, form));

		}

		// Actually add those generated variants to the asset manager.
		foreach (MetaAsset metaAsset in species.metaAssets.ToArray()) // ToArray() is used to keep the enumerator valid while editing species.metaAssets
		{

			// Add the variant itself. This should be unique, so crash if there's a duplicate.
			assetManager.Add(await metaAsset.CreateAsset());

		}

	}

	/// <summary>
	/// <para>Builds one MetaVariant if no gender difference is required for this variant or it is already implied to have one.</para>
	/// <para>Otherwise, builds as many as needed for gender differences (2).</para>
	/// </summary>
	/// <param name="species">The species this variant belongs to.</param>
	/// <param name="baseForm">The MetaForm to build a variant based on, may already be gendered.</param>
	/// <returns>An enumerable of MetaVariants, should have either 1 or 2 variants.</returns>
	private static async Task<IEnumerable<MetaVariant>> BuildGenderedVariants(MetaSpecies species, MetaForm baseForm)
	{

		IEnumerable<MetaForm> potentialForms = species.metaAssets.OfType<MetaForm>();

		if (baseForm.genderAlignment != GenderAlignment.None)
			return [new(species, baseForm, baseForm.genderAlignment)];

		if (BuildSpeciesUtils.WouldNoneAlignmentNeedGenderDifferences(baseForm, potentialForms))
		{

			GenderAlignment[] genders = [GenderAlignment.Female, GenderAlignment.Male];
			List<MetaVariant> variants = [];

			foreach (GenderAlignment gender in genders)
			{
				MetaVariant variant = new(species, baseForm, gender);
				variant.Name += "-" + BuildSpeciesUtils.GetGenderFilenameDistinction(gender);
				variants.Add(variant);
			}

			return variants;

		}

		return [new(species, baseForm, GenderAlignment.None)];

	}

	/// <summary>
	/// <para>Highly opinionated function to attempt to match <see cref="MetaVisual"/>s with the provided <see cref="MetaForm"/></para>
	/// </summary>
	/// <param name="form">The form to match <see cref="MetaVisual"/>s to.</param>
	/// <param name="species">The <see cref="MetaSpecies"/> to retrieve <see cref="MetaVisual"/>s from.</param>
	/// <returns>An enumerable of <see cref="MetaVisual"/>s from <paramref name="species"/> that likely match <paramref name="form"/>.</returns>
	/// <remarks>
	/// <para>Improvements to the matching algorithm are welcome.</para>
	/// </remarks>
	private static async Task<IEnumerable<MetaVisual>> GetClosestVisualMatches(MetaForm form, MetaSpecies species)
	{

		int highestMatchResult = int.MinValue; // Matches
		int lowestSpecificityTiebreaker = int.MaxValue; // Tie-breaker, so lower counts with the same match count are prioritized.
		List<MetaVisual> foundVisuals = [];

		foreach (MetaVisual visual in species.metaAssets.OfType<MetaVisual>())
		{

			if (!BuildSpeciesUtils.IsConnectableGender(form.genderAlignment, visual.genderAlignment))
				continue;

			int specificity = visual.GetMatchableParts().Count();
			int matchResults = INameMatchable.CalculateNameMatches(form, visual);

			// If this is not the best match, leave.
			if (matchResults < highestMatchResult)
				continue;

			// If this is tied for the best match, but more specific, leave.
			if (matchResults == highestMatchResult && specificity > lowestSpecificityTiebreaker)
				continue;

			// If this is the new best match (not a tie), wipe the list.
			if (matchResults > highestMatchResult)
				foundVisuals.Clear();

			foundVisuals.Add(visual);
			highestMatchResult = matchResults;
			lowestSpecificityTiebreaker = specificity;
			continue;

		}

		return foundVisuals;

	}

}
