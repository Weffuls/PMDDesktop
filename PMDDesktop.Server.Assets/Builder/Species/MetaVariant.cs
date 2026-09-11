using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Holds a variant and metadata about it.
/// </summary>
/// <param name="variant">The variant we're talking about.</param>
internal class MetaVariant : MetaAsset
{

	internal MetaVariant(MetaSpecies species, MetaForm baseForm, GenderAlignment genderAlignment)
	{

		IEnumerable<MetaForm> potentialForms = species.metaAssets.OfType<MetaForm>().Where(form => baseForm != form);

		ExtraForms = [.. BuildSpeciesUtils.EnumerateLinkableForms(baseForm, potentialForms, genderAlignment)];

		Name = baseForm.Name;
		OriginalNames = baseForm.OriginalNames;

		Species = species;
		BaseForm = baseForm;

		GenderAlignment = genderAlignment;

	}

	internal GenderAlignment GenderAlignment { get; set; }
	internal MetaSpecies Species { get; set; }
	internal MetaForm BaseForm { get; set; }
	internal List<MetaForm> ExtraForms { get; set; }
	internal string Name { get; set; }
	internal List<string> OriginalNames { get; init; }

	internal override AssetLocation Location => new(Species.Location, "variants", Name);

	internal override async Task<Asset> CreateAsset()
	{

		return new SpeciesVariant(Location)
		{
			Species = new(Species.Location),
			EvolutionTags = [],
			DefaultForm = new(BaseForm.Location),
			OtherForms = [.. ExtraForms.Select((form) => new AssetReference<SpeciesForm>(form.Location))]
		};

	}

}
