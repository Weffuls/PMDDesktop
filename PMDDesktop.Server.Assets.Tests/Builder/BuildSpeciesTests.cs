using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Tests.Builder;

public class BuildSpeciesTests(BuildSpeciesFixture fixture) : IClassFixture<BuildSpeciesFixture>
{

	private BuildSpeciesFixture fixture = fixture;

	#region Helper Functions

	private bool DoesVariantReferenceForm(SpeciesVariant variant, SpeciesForm form)
	{

		if (variant.DefaultForm.GetReference(fixture.assets) == form)
			return true;

		return variant.OtherForms.Any((reference) => reference.GetReference(fixture.assets) == form);

	}

	private void GetVariants(string speciesName, out IEnumerable<SpeciesVariant> variants)
	{

		variants = fixture.assets.OfType<SpeciesVariant>().Where(variant => (string)variant.Species.Location.Last() == speciesName);

		Assert.Distinct(variants);

	}

	private void GetVariantsAndForms(string speciesName, out IEnumerable<SpeciesVariant> variants, out IEnumerable<SpeciesForm> forms)
	{

		GetVariants(speciesName, out IEnumerable<SpeciesVariant> foundVariants);

		variants = foundVariants; // Workaround for CS1628.
		forms = fixture.assets.OfType<SpeciesForm>().Where(form => foundVariants.Any(variant => DoesVariantReferenceForm(variant, form)));

	}

	#endregion Helper Functions

	#region Existence

	[Fact]
	public void CanReadMockZip()
	{

		Assert.NotNull(fixture.apiZip);
		Assert.NotNull(fixture.spriteZip);

	}

	[Fact]
	public void ContainsAtLeastOneSpecies()
	{
		Assert.NotEmpty(fixture.assets.OfType<Species>());
	}

	[Fact]
	public void ContainsAtLeastOneSpeciesVariant()
	{
		Assert.NotEmpty(fixture.assets.OfType<Species>());
	}

	[Fact]
	public void ContainsAtLeastOneSpeciesForm()
	{
		Assert.NotEmpty(fixture.assets.OfType<Species>());
	}

	#endregion Existence

	#region Linked Data

	[Fact]
	public void AllSpeciesHaveAVariant()
	{

		IEnumerable<SpeciesVariant> variants = fixture.assets.OfType<SpeciesVariant>();

		Assert.All(fixture.assets.OfType<Species>(), (species) => Assert.Contains(variants, (variant) => variant.Species.GetReference(fixture.assets) == species));

	}

	[Fact]
	public void AllVariantsHaveASpecies()
	{

		Assert.All(fixture.assets.OfType<SpeciesVariant>(), (variant) => Assert.NotNull(variant.Species.GetReference(fixture.assets)));

	}

	[Fact]
	public void AllVariantsHaveADefaultForm()
	{

		Assert.All(fixture.assets.OfType<SpeciesVariant>(), (variant) => Assert.NotNull(variant.DefaultForm.GetReference(fixture.assets)));

	}

	[Fact]
	public void AllFormsHaveAVariant()
	{

		IEnumerable<SpeciesVariant> variants = fixture.assets.OfType<SpeciesVariant>();

		Assert.All(fixture.assets.OfType<SpeciesForm>(), (form) => Assert.Contains(variants, (variant) => DoesVariantReferenceForm(variant, form)));

	}

	[Fact]
	public void AllFormsHavePortraits()
	{

		IEnumerable<SpeciesPortraits> normalPortraits = fixture.assets.OfType<SpeciesPortraits>().Where(visual => !visual.Shiny);
		IEnumerable<SpeciesPortraits> shinyPortraits = fixture.assets.OfType<SpeciesPortraits>().Where(visual => visual.Shiny);

		IEnumerable<SpeciesForm> forms = fixture.assets.OfType<SpeciesForm>();

		Assert.All(forms, form => Assert.Contains(normalPortraits, portrait => portrait.ForForms.Any(forForm => forForm.Location == form.Path)));
		Assert.All(forms, form => Assert.Contains(shinyPortraits, portrait => portrait.ForForms.Any(forForm => forForm.Location == form.Path)));

	}

	[Fact]
	public void AllFormsHaveSprites()
	{

		IEnumerable<SpeciesSprites> normalSprites = fixture.assets.OfType<SpeciesSprites>().Where(visual => !visual.Shiny);
		IEnumerable<SpeciesSprites> shinySprites = fixture.assets.OfType<SpeciesSprites>().Where(visual => visual.Shiny);

		IEnumerable<SpeciesForm> forms = fixture.assets.OfType<SpeciesForm>();

		Assert.All(forms, form => Assert.Contains(normalSprites, portrait => portrait.ForForms.Any(forForm => forForm.Location == form.Path)));
		Assert.All(forms, form => Assert.Contains(shinySprites, portrait => portrait.ForForms.Any(forForm => forForm.Location == form.Path)));

	}

	[Fact]
	public void AllVariantsListUniqueForms()
	{

		// No variant should reference the same form twice.

		IEnumerable<SpeciesVariant> variants = fixture.assets.OfType<SpeciesVariant>();

		Assert.All(variants, variant => Assert.Distinct([.. variant.OtherForms.Select(reference => reference.Location), variant.DefaultForm.Location]));

	}

	[Fact]
	public void AllEvolutionsHaveValidReferences()
	{

		Assert.All(fixture.assets.OfType<Species>(), (species) =>
		{

			if (species.EvolvesFrom is null)
				return;

			Assert.NotNull(species.EvolvesFrom.FromSpecies.GetReference(fixture.assets));

		});

	}

	#endregion Linked Data

	#region Bulbasaur/Ivysaur/Venusaur Checks

	[Fact]
	public void BulbasaurEvolutionLine()
	{

		Species bulbasaur = fixture.assets.OfType<Species>().First(species => "0001-bulbasaur"u8.SequenceEqual(species.Path.Last()));
		Species ivysaur = fixture.assets.OfType<Species>().First(species => "0002-ivysaur"u8.SequenceEqual(species.Path.Last()));
		Species venusaur = fixture.assets.OfType<Species>().First(species => "0003-venusaur"u8.SequenceEqual(species.Path.Last()));

		Assert.Null(bulbasaur.EvolvesFrom);

		Assert.NotNull(ivysaur.EvolvesFrom);
		Assert.Equal(bulbasaur, ivysaur.EvolvesFrom.FromSpecies.GetReference(fixture.assets));
		Assert.NotEqual(0, ivysaur.EvolvesFrom.MinimumLevel);

		Assert.NotNull(venusaur.EvolvesFrom);
		Assert.Equal(ivysaur, venusaur.EvolvesFrom.FromSpecies.GetReference(fixture.assets));
		Assert.NotEqual(0, venusaur.EvolvesFrom.MinimumLevel);

	}

	[Fact]
	public void VenusaurChecks()
	{

		// Has gender differences, so these should be seperate.
		SpeciesVariant maleVenusaur = fixture.assets.OfType<SpeciesVariant>().First(variant => "venusaur-male"u8.SequenceEqual(variant.Path.Last()));
		SpeciesVariant femaleVenusaur = fixture.assets.OfType<SpeciesVariant>().First(variant => "venusaur-female"u8.SequenceEqual(variant.Path.Last()));

		// Default forms should be different.
		Assert.NotEqual(maleVenusaur.DefaultForm.Location, femaleVenusaur.DefaultForm.Location);

		// Both should reference venusaur-mega
		SpeciesForm megaVenusaur = fixture.assets.OfType<SpeciesForm>().First(form => "venusaur-mega"u8.SequenceEqual(form.Path.Last()));
		Assert.True(DoesVariantReferenceForm(maleVenusaur, megaVenusaur));
		Assert.True(DoesVariantReferenceForm(femaleVenusaur, megaVenusaur));

		// Both should reference venusaur-gmax
		SpeciesForm gmaxVenusaur = fixture.assets.OfType<SpeciesForm>().First(form => "venusaur-gmax"u8.SequenceEqual(form.Path.Last()));
		Assert.True(DoesVariantReferenceForm(maleVenusaur, gmaxVenusaur));
		Assert.True(DoesVariantReferenceForm(femaleVenusaur, gmaxVenusaur));

	}

	#endregion Bulbasaur/Ivysaur/Venusaur Checks

	#region Espurr/Meowstic Checks

	[Fact]
	public void EspurrEvolutionLine()
	{

		Species espurr = fixture.assets.OfType<Species>().First(species => "0677-espurr"u8.SequenceEqual(species.Path.Last()));
		Species meowstic = fixture.assets.OfType<Species>().First(species => "0678-meowstic"u8.SequenceEqual(species.Path.Last()));

		Assert.Null(espurr.EvolvesFrom);

		Assert.NotNull(meowstic.EvolvesFrom);
		Assert.Equal(espurr, meowstic.EvolvesFrom.FromSpecies.GetReference(fixture.assets));
		Assert.NotEqual(0, meowstic.EvolvesFrom.MinimumLevel);

	}

	[Fact]
	public void MeowsticChecks()
	{

		// Has gender differences, so these should be seperate.
		// Unlike Venusaur, these gender differences are already baked into the API, so this is testing that they don't get split again.
		SpeciesVariant maleMeowstic = fixture.assets.OfType<SpeciesVariant>().First(variant => "meowstic-male"u8.SequenceEqual(variant.Path.Last()));
		SpeciesVariant femaleMeowstic = fixture.assets.OfType<SpeciesVariant>().First(variant => "meowstic-female"u8.SequenceEqual(variant.Path.Last()));

		// Default forms should be different.
		Assert.NotEqual(maleMeowstic.DefaultForm.Location, femaleMeowstic.DefaultForm.Location);

		// Both should reference meowstic-mega
		// They are seperate in the API, but they should be merged in the final asset build, as they do not have unique properties.
		SpeciesForm megaMeowstic = fixture.assets.OfType<SpeciesForm>().First(form => "meowstic-mega"u8.SequenceEqual(form.Path.Last()));
		Assert.True(DoesVariantReferenceForm(maleMeowstic, megaMeowstic));
		Assert.True(DoesVariantReferenceForm(femaleMeowstic, megaMeowstic));

		// There should only be 2 Meowstic Variants
		GetVariants("0678-meowstic", out IEnumerable<SpeciesVariant> variants);
		Assert.Equal(2, variants.Count());

	}

	#endregion Espurr/Meowstic Checks

	#region Lurantis Checks

	[Fact]
	public void FomantisEvolutionLine()
	{

		Species fomantis = fixture.assets.OfType<Species>().First(species => "0753-fomantis"u8.SequenceEqual(species.Path.Last()));
		Species lurantis = fixture.assets.OfType<Species>().First(species => "0754-lurantis"u8.SequenceEqual(species.Path.Last()));

		Assert.Null(fomantis.EvolvesFrom);

		Assert.NotNull(lurantis.EvolvesFrom);
		Assert.Equal(fomantis, lurantis.EvolvesFrom.FromSpecies.GetReference(fixture.assets));
		Assert.NotEqual(0, lurantis.EvolvesFrom.MinimumLevel);

	}

	[Fact]
	public void TotemLurantisIsRemoved()
	{

		GetVariantsAndForms("0754-lurantis", out IEnumerable<SpeciesVariant> variants, out IEnumerable<SpeciesForm> forms);

		// Since lurantis-totem doesn't have unique stats or spritework, it shouldn't be included.

		Assert.DoesNotContain(variants, variant => ((string)variant.Path.Last()).Contains("totem"));
		Assert.DoesNotContain(forms, form => ((string)form.Path.Last()).Contains("totem"));

		// There should only be 1 Lurantis form/variant
		Assert.Single(variants);
		Assert.Single(forms);

	}

	#endregion Lurantis Checks

	#region Scatterbug/Spewpa/Vivillon Checks

	private static readonly string[] VIVILLON_NAME_SUFFIXES = ["meadow", "icy-snow", "polar", "tundra", "continental", "garden", "elegant", "modern", "marine", "archipelago", "high-plains", "sandstorm", "river", "monsoon", "savanna", "sun", "ocean", "jungle", "fancy", "poke-ball"];

	private static IEnumerable<string> GetScatterbugAPINames() => VIVILLON_NAME_SUFFIXES.Select(suffix => "scatterbug-" + suffix);
	private static IEnumerable<string> GetSpewpaAPINames() => VIVILLON_NAME_SUFFIXES.Select(suffix => "spewpa-" + suffix);
	private static IEnumerable<string> GetVivillonAPINames() => VIVILLON_NAME_SUFFIXES.Select(suffix => "vivillon-" + suffix);

	[Fact]
	public void ScatterbugEvolutionLine()
	{

		Species scatterbug = fixture.assets.OfType<Species>().First(species => "0664-scatterbug"u8.SequenceEqual(species.Path.Last()));
		Species spewpa = fixture.assets.OfType<Species>().First(species => "0665-spewpa"u8.SequenceEqual(species.Path.Last()));
		Species vivillon = fixture.assets.OfType<Species>().First(species => "0666-vivillon"u8.SequenceEqual(species.Path.Last()));

		Assert.Null(scatterbug.EvolvesFrom);

		Assert.NotNull(spewpa.EvolvesFrom);
		Assert.Equal(scatterbug, spewpa.EvolvesFrom.FromSpecies.GetReference(fixture.assets));
		Assert.NotEqual(0, spewpa.EvolvesFrom.MinimumLevel);

		Assert.NotNull(vivillon.EvolvesFrom);
		Assert.Equal(spewpa, vivillon.EvolvesFrom.FromSpecies.GetReference(fixture.assets));
		Assert.NotEqual(0, vivillon.EvolvesFrom.MinimumLevel);

	}

	[Fact]
	public void ScatterbugFormsMerged()
	{

		// Since each Scatterbug form is NOT unique (no unique artwork), they should be merged into one.
		Assert.All(fixture.assets, asset => Assert.DoesNotContain(GetScatterbugAPINames(), name => name == (string)asset.Path.Last()));

		// There should only be one variant & form.
		GetVariantsAndForms("0664-scatterbug", out IEnumerable<SpeciesVariant> variants, out IEnumerable<SpeciesForm> forms);
		Assert.Single(variants);
		Assert.Single(forms);

		// That single variant & form's name should be simple.
		Assert.Contains(fixture.assets.OfType<SpeciesForm>(), form => "scatterbug"u8.SequenceEqual(form.Path.Last()));
		Assert.Contains(fixture.assets.OfType<SpeciesVariant>(), variant => "scatterbug"u8.SequenceEqual(variant.Path.Last()));

	}

	[Fact]
	public void SpewpaFormsMerged()
	{

		// Since each Spewpa form is NOT unique (no unique artwork), they should be merged into one.

		Assert.All(fixture.assets, asset => Assert.DoesNotContain(GetSpewpaAPINames(), name => name == (string)asset.Path.Last()));

		// There should only be one variant & form.
		GetVariantsAndForms("0665-spewpa", out IEnumerable<SpeciesVariant> variants, out IEnumerable<SpeciesForm> forms);
		Assert.Single(variants);
		Assert.Single(forms);

		// That single variant & form's name should be simple.
		Assert.Contains(fixture.assets.OfType<SpeciesForm>(), form => "spewpa"u8.SequenceEqual(form.Path.Last()));
		Assert.Contains(fixture.assets.OfType<SpeciesVariant>(), form => "spewpa"u8.SequenceEqual(form.Path.Last()));

	}

	[Fact]
	public void VivillonFormsSeperated()
	{

		GetVariantsAndForms("0666-vivillon", out IEnumerable<SpeciesVariant> variants, out IEnumerable<SpeciesForm> forms);

		// There should not be any more or less than expected. (20)
		Assert.Equal(GetVivillonAPINames().Count(), variants.Count());
		Assert.Equal(GetVivillonAPINames().Count(), forms.Count());

		// Since each Vivillon form is unique (has unique artwork), they should be independently selectable.
		Assert.All(GetVivillonAPINames(), name => Assert.Contains(forms, form => (string)form.Path.Last() == name));
		Assert.All(GetVivillonAPINames(), name => Assert.Contains(variants, variant => (string)variant.Path.Last() == name));

		// There should not be a standalone, variantless version.
		Assert.DoesNotContain(fixture.assets.OfType<SpeciesForm>(), form => "vivillon"u8.SequenceEqual(form.Path.Last()));
		Assert.DoesNotContain(fixture.assets.OfType<SpeciesVariant>(), form => "vivillon"u8.SequenceEqual(form.Path.Last()));

		// They should each reference a unique base form.
		Assert.Distinct(variants.Select(variant => variant.DefaultForm.Location));

		// They should not have any other forms.
		Assert.All(variants, variant => Assert.Empty(variant.OtherForms));

	}

	[Fact]
	public void VivillonFormsHaveUniqueArtwork()
	{

		List<SpeciesVisual> seenVisuals = [];

		GetVariantsAndForms("0666-vivillon", out IEnumerable<SpeciesVariant> variants, out IEnumerable<SpeciesForm> forms);

		foreach (SpeciesForm form in forms)
		{

			IEnumerable<SpeciesVisual> visualsPointingTo = fixture.assets.OfType<SpeciesVisual>().Where(visual => visual.ForForms.Any(forForm => forForm.Location == form.Path));
			IEnumerable<SpeciesPortraits> portraits = visualsPointingTo.OfType<SpeciesPortraits>();
			IEnumerable<SpeciesSprites> sprites = visualsPointingTo.OfType<SpeciesSprites>();

			// Should contain a normal portrait.
			Assert.Contains(portraits, visual => !visual.Shiny);

			// Should contain a shiny portrait.
			Assert.Contains(portraits, visual => visual.Shiny);

			// Should contain a normal sprite.
			Assert.Contains(sprites, visual => !visual.Shiny);

			// Should contain a shiny sprite.
			Assert.Contains(sprites, visual => visual.Shiny);

			seenVisuals.AddRange(visualsPointingTo);

		}

		Assert.Distinct(seenVisuals);

	}

	#endregion Scatterbug/Spewpa/Vivillon Checks

}
