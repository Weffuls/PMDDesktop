using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Object for holding Metadata during the <see cref="BuildSpecies"/> routine about an <see cref="SpeciesVariant"/> before creating it.
/// </summary>
internal class MetaVariant : MetaAsset
{

	internal MetaVariant(MetaSpecies species, MetaForm baseForm, GenderAlignment genderAlignment)
	{

		IEnumerable<MetaForm> potentialForms = species.metaAssets.OfType<MetaForm>().Where(form => baseForm != form);

		ExtraForms = [.. BuildSpeciesUtils.EnumerateLinkableForms(baseForm, potentialForms, genderAlignment)];

		Name = baseForm.Name;

		Species = species;
		BaseForm = baseForm;

		GenderAlignment = genderAlignment;

	}

	/// <summary>
	/// The gender that this variant is aligning to. Helps with portraying gender differences. Linked forms should match the alignment or be <see cref="GenderAlignment.None"/>
	/// </summary>
	internal GenderAlignment GenderAlignment { get; set; }

	/// <summary>
	/// The species that this <see cref="MetaVariant"/> belongs to.
	/// </summary>
	internal MetaSpecies Species { get; set; }

	/// <summary>
	/// The (standalone) <see cref="MetaForm"/> that this variant was directly created for.
	/// </summary>
	internal MetaForm BaseForm { get; set; }

	/// <summary>
	/// <see cref="MetaForm"/>s that this Variant could also turn into under certain conditions.
	/// </summary>
	internal List<MetaForm> ExtraForms { get; set; }

	/// <summary>
	/// The name of this <see cref="MetaVariant"/>. Also used to determine the <see cref="Location"/>.
	/// </summary>
	internal string Name { get; set; }

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
