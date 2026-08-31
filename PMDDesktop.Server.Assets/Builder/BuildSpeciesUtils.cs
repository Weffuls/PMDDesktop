namespace PMDDesktop.Server.Assets.Builder;

internal static class BuildSpeciesUtils
{

	public static bool WouldMakeLink(MetaForm baseForm, MetaForm target, GenderAlignment genderAlignment)
	{

		if (baseForm == target)
			return false;

		if (PokeApiUtils.IsPokemonFormStandalone(target.formRoot))
			return false;

		if (!IsConnectableGender(genderAlignment, target.genderAlignment))
			return false;

		if (!PokeApiUtils.IsPokemonFormWithMatchingBaseForm(target.formRoot, [baseForm.originalFormName], true))
			return false;

		return true;

	}

	/// <summary>
	/// Would this baseForm need 
	/// </summary>
	/// <param name="baseForm"></param>
	/// <param name="potentialForms"></param>
	/// <returns></returns>
	public static bool WouldNoneAlignmentNeedGenderDifferences(MetaForm baseForm, IEnumerable<MetaForm> potentialForms)
	{

		if (baseForm.genderAlignment != GenderAlignment.None)
			throw new InvalidOperationException($"{baseForm} is not gender neutral, it is aligned to {Enum.GetName(baseForm.genderAlignment)}, which means the gender differences have already been decided or could be inferred.");

		bool sawFemaleForm = false;
		bool sawMaleForm = false;

		foreach (MetaForm linkableForm in EnumerateLinkableForms(baseForm, potentialForms, GenderAlignment.None))
		{

			if (linkableForm.genderAlignment == GenderAlignment.Female)
				sawFemaleForm = true;
			else if (linkableForm.genderAlignment == GenderAlignment.Male)
				sawMaleForm = true;

			if (sawFemaleForm && sawMaleForm)
				return true;

		}

		return false;

	}

	/// <summary>
	/// Could these genders connect to each other? That is to say, is either argument not none and different?
	/// </summary>
	/// <param name="left">1st gender to check connectability with</param>
	/// <param name="right">2nd gender to check connectability with</param>
	/// <returns>True if they can be connected, false otherwise.</returns>
	public static bool IsConnectableGender(GenderAlignment left, GenderAlignment right)
	{

		if (left == GenderAlignment.None)
			return true;

		if (right == GenderAlignment.None)
			return true;

		if (left == right)
			return true;

		return false;

	}

	/// <summary>
	/// Return a string that could be included in a form name to distinguish it from the other gender's forms.
	/// </summary>
	/// <param name="gender">The gender to get the name of.</param>
	/// <returns>The name that could be appended a part of a form name.</returns>
	/// <exception cref="InvalidOperationException">Throws if it recieves anything other than Male or Female.</exception>
	/// <remarks>Will not accept None as a gender. It doesn't make sense for this use case.</remarks>
	public static string GetGenderFilenameDistinction(GenderAlignment gender)
	{

		if (gender == GenderAlignment.Male)
			return "male";

		if (gender == GenderAlignment.Female)
			return "female";

		throw new InvalidOperationException($"{nameof(GetGenderFilenameDistinction)} was called with a value that wasn't Male or Female, which does not make sense for its use case.");

	}

	public static IEnumerable<MetaForm> EnumerateLinkableForms(MetaForm baseForm, IEnumerable<MetaForm> potentialForms, GenderAlignment genderAlignment)
	{

		foreach (MetaForm potentialForm in potentialForms)
		{

			if (potentialForm == baseForm)
				continue;

			bool linkable = WouldMakeLink(baseForm, potentialForm, genderAlignment);

			if (linkable)
				yield return potentialForm;

		}

	}

}
