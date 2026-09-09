using PMDDesktop.Server.Assets.Builder.ZipScavenger;
using System.IO.Compression;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.BuildSteps;

internal static class BuildSpecies
{

	public static async Task StartBuildStep(AssetManager assets)
	{

		using PokeApiZip apiZip = await ZipManager.GetPokeApiZip();
		using SpriteCollabZip spriteZip = await ZipManager.GetSpriteCollabZip();

		await BuildTopLevelSpecies(assets, apiZip, spriteZip);

		return;

	}

	public static async Task BuildWithCustomZips(AssetManager assets, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		await BuildTopLevelSpecies(assets, apiZip, spriteZip);

		return;

	}

	private static async Task BuildTopLevelSpecies(AssetManager assets, PokeApiZip apiZip, SpriteCollabZip spriteZip)
	{

		foreach (ZipArchiveEntry entry in apiZip.EnumerateSpecies())
		{

			using Stream stream = await entry.OpenAsync();

			using JsonDocument json = await JsonDocument.ParseAsync(stream);

			await BuildFullSpeciesAndVariantsAndForms(assets, json.RootElement, apiZip, spriteZip);

		}

	}

	private static async Task BuildFullSpeciesAndVariantsAndForms(AssetManager assets, JsonElement pokemonSpeciesRoot, PokeApiZip apiZip, SpriteCollabZip spriteZip)
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
			assets.Add(await metaAsset.CreateAsset());

		}

	}

	/// <summary>
	/// Builds one MetaVariant if no gender difference is required for this variant or it is already implied to have one.
	/// Otherwise, builds as many as needed for gender differences (2).
	/// </summary>
	/// <param name="species">The species this variant belongs to.</param>
	/// <param name="baseForm">The MetaForm to build a variant based on, may already be gendered.</param>
	/// <param name="potentialForms">All forms that this variant can have, may include the baseForm.</param>
	/// <param name="zip">Any PokeApiZip to read from when needed.</param>
	/// <returns>An enumerable of MetaVariants.</returns>
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
				variants.Add(new(species, baseForm, gender));
			}

			return variants;

		}

		return [new(species, baseForm, GenderAlignment.None)];

	}

	private static async Task<IEnumerable<MetaVisual>> GetClosestVisualMatches(MetaForm form, MetaSpecies species)
	{

		int highestMatchResult = 0; // Matches
		int lowestSpecificityTiebreaker = 0; // Tie-breaker, so lower counts with the same match count are prioritized.
		List<MetaVisual> foundVisuals = [];

		foreach (MetaVisual visual in species.metaAssets.OfType<MetaVisual>())
		{

			if (!BuildSpeciesUtils.IsConnectableGender(form.genderAlignment, visual.genderAlignment))
				continue;

			int specificity = visual.GetMatchableParts().Count();
			int matchResults = INameMatchable.CalculateNameMatches(form, visual);

			if (matchResults >= highestMatchResult)
			{
				if (matchResults > highestMatchResult)
					foundVisuals.Clear();

				foundVisuals.Add(visual);
				highestMatchResult = matchResults;
				lowestSpecificityTiebreaker = specificity;
				continue;
			}

			if (matchResults == highestMatchResult && specificity <= lowestSpecificityTiebreaker)
			{
				if (specificity < lowestSpecificityTiebreaker)
					foundVisuals.Clear();

				foundVisuals.Add(visual);
				highestMatchResult = matchResults;
				lowestSpecificityTiebreaker = specificity;
				continue;
			}

		}

		return foundVisuals;

	}

}
