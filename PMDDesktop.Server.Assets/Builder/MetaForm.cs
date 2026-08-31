using PMDDesktop.Server.Assets.Data;
using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder;

/// <summary>
/// Holds a form and metadata about it, such as the requirements to change into this form.
/// </summary>
/// <param name="form">The form we're talking about.</param>
internal class MetaForm()
{
	public required JsonElement formRoot;
	public required string originalFormName;
	public required SpeciesForm form;

	public GenderAlignment genderAlignment = GenderAlignment.None;

	public bool IsStandaloneForm()
	{

		return PokeApiUtils.IsPokemonFormStandalone(formRoot);

	}

	public static implicit operator SpeciesForm(MetaForm meta) => meta.form;
}
