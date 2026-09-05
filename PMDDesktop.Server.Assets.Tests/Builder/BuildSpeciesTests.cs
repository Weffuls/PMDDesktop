using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Tests.Builder;

public class BuildSpeciesTests(BuildSpeciesFixture fixture) : IClassFixture<BuildSpeciesFixture>
{

	private BuildSpeciesFixture fixture = fixture;

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

		Assert.All(forms, form => Assert.Contains(normalPortraits, portrait => portrait.forForms.Any(forForm => forForm.Location == form.Location)));
		Assert.All(forms, form => Assert.Contains(shinyPortraits, portrait => portrait.forForms.Any(forForm => forForm.Location == form.Location)));

	}

	[Fact]
	public void AllFormsHaveSprites()
	{

		IEnumerable<SpeciesSprites> normalSprites = fixture.assets.OfType<SpeciesSprites>().Where(visual => !visual.Shiny);
		IEnumerable<SpeciesSprites> shinySprites = fixture.assets.OfType<SpeciesSprites>().Where(visual => visual.Shiny);

		IEnumerable<SpeciesForm> forms = fixture.assets.OfType<SpeciesForm>();

		Assert.All(forms, form => Assert.Contains(normalSprites, portrait => portrait.forForms.Any(forForm => forForm.Location == form.Location)));
		Assert.All(forms, form => Assert.Contains(shinySprites, portrait => portrait.forForms.Any(forForm => forForm.Location == form.Location)));

	}

	private bool DoesVariantReferenceForm(SpeciesVariant variant, SpeciesForm form)
	{

		if (variant.DefaultForm.GetReference(fixture.assets) == form)
			return true;

		return variant.OtherForms.Any((reference) => reference.GetReference(fixture.assets) == form);

	}

	#endregion Linked Data

}
