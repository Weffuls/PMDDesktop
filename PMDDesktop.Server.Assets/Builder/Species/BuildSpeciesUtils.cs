using System.Text.Json;

namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Helper functions to assist with the function of <see cref="BuildSpecies"/> while keeping code easy to follow.
/// </summary>
internal static class BuildSpeciesUtils
{

	/// <summary>
	/// <para>Opinionated function to test if one <see cref="MetaForm"/> can link to another <see cref="MetaForm"/>.</para>
	/// <para>If <paramref name="baseForm"/> can link to <paramref name="target"/> (this function returns true), then you can add <paramref name="target"/> to the <see cref="MetaVariant.ExtraForms"/> property in the <see cref="MetaVariant"/> that is based on or has a connection to <paramref name="baseForm"/>.</para>
	/// </summary>
	/// <param name="baseForm"><see cref="MetaForm"/> that is testing linkability to <paramref name="target"/>.</param>
	/// <param name="target"><see cref="MetaForm"/> that <paramref name="baseForm"/> is testing linkability to.</param>
	/// <param name="genderAlignment"><see cref="GenderAlignment"/> of <paramref name="baseForm"/> to use when linking with a gendered <paramref name="target"/>. Useful when polling for gender differences, otherwise let it match <paramref name="baseForm"/>'s <see cref="MetaForm.genderAlignment"/></param>
	/// <returns>True if they can be linked, otherwise false.</returns>
	public static bool WouldMakeLink(MetaForm baseForm, MetaForm target, GenderAlignment genderAlignment)
	{

		// Can't link to itself.
		if (baseForm == target)
			return false;

		// Can only link to a form that is not standalone.
		if (target.IsStandaloneForm())
			return false;

		// Can only link if the gender alignments are connectable.
		if (!IsConnectableGender(genderAlignment, target.genderAlignment))
			return false;

		// Can only link if one of the target's form roots includes us as a base form OR does not mention any base forms.
		if (!AnyPokemonFormWithMatchingBaseForm(target.FormRoots, baseForm.OriginalNames, true))
			return false;

		return true;

	}

	/// <summary>
	/// <para>Checks if any provided <paramref name="formRoots"/> match any provided <paramref name="names"/> as a "base_form".</para>
	/// <para>See <see cref="PokeApiUtils.IsPokemonFormWithMatchingBaseForm(JsonElement, IEnumerable{string}, bool)"/> for more information.</para>
	/// </summary>
	/// <param name="formRoots">An enumerable of <see cref="JsonElement"/>s of the root of a "pokemon-form".</param>
	/// <param name="names">The names you'd like to match with "base_form".</param>
	/// <param name="returnTrueIfNull">If there are no listed base forms on <b>any singular</b> provided element in <paramref name="formRoots"/>, should this function return true?</param>
	/// <returns></returns>
	private static bool AnyPokemonFormWithMatchingBaseForm(IEnumerable<JsonElement> formRoots, IEnumerable<string> names, bool returnTrueIfNull)
	{

		foreach (JsonElement root in formRoots)
			if (PokeApiUtils.IsPokemonFormWithMatchingBaseForm(root, names, returnTrueIfNull))
				return true;

		return false;

	}

	/// <summary>
	/// <para>Test if this <b><see cref="GenderAlignment.None"/></b> <paramref name="baseForm"/> would need gender differences if it attempted to link to all its forms.</para>
	/// <para>In other words, can <paramref name="baseForm"/> link to both <see cref="GenderAlignment.Male"/> and <see cref="GenderAlignment.Female"/> <see cref="MetaForm"/>s in <paramref name="potentialForms"/>?</para>
	/// </summary>
	/// <param name="baseForm">The <see cref="MetaForm"/> to be used as the starting point for linking to the provided <paramref name="potentialForms"/>.</param>
	/// <param name="potentialForms">The <see cref="MetaForm"/>s that <paramref name="baseForm"/> will be attempting to link to.</param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException">If <paramref name="baseForm"/> is not <see cref="GenderAlignment.None"/>.</exception>
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
	/// Could these genders connect to each other? That is to say, is either argument not <see cref="GenderAlignment.None"/> and different?
	/// </summary>
	/// <param name="left">1st <see cref="GenderAlignment"/> to check connectability with.</param>
	/// <param name="right">2nd <see cref="GenderAlignment"/> to check connectability with.</param>
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
	/// <para>Returns a string that could be included in a form name that was split for gender differences to distinguish it from the other gender's forms.</para>
	/// <para>This is usually "-male" or "-female"</para>
	/// </summary>
	/// <param name="gender">The gender to get the name of.</param>
	/// <returns>The name that could be appended a part of a form name.</returns>
	/// <exception cref="InvalidOperationException">Throws if it recieves anything other than Male or Female.</exception>
	/// <remarks>Will not accept <see cref="GenderAlignment.None"/> as a gender. It doesn't make sense for this use case, so the call should be avoided.</remarks>
	public static string GetGenderFilenameDistinction(GenderAlignment gender)
	{

		if (gender == GenderAlignment.Male)
			return "male";

		if (gender == GenderAlignment.Female)
			return "female";

		throw new InvalidOperationException($"{nameof(GetGenderFilenameDistinction)} was called with a value that wasn't Male or Female, which does not make sense for its use case.");

	}

	/// <summary>
	/// <para>If <paramref name="checkString"/> is "male" after normalization, returns <see cref="GenderAlignment.Male"/></para>
	/// <para>If <paramref name="checkString"/> is "female" after normalization, returns <see cref="GenderAlignment.Female"/></para>
	/// </summary>
	/// <param name="checkString">The string to examine.</param>
	/// <returns><see cref="GenderAlignment.Male"/> if <paramref name="checkString"/> is "male", <see cref="GenderAlignment.Female"/> if <paramref name="checkString"/> is "female", otherwise <see cref="GenderAlignment.None"/></returns>
	public static GenderAlignment IsGenderName(string checkString)
	{

		return INameMatchable.NormalizeStringForMatching(checkString) switch
		{
			"female" => GenderAlignment.Female,
			"male" => GenderAlignment.Male,
			_ => GenderAlignment.None
		};

	}

	/// <summary>
	/// <para>If any string in <paramref name="checkStrings"/> is "male" after normalization, returns <see cref="GenderAlignment.Male"/></para>
	/// <para>If any string in <paramref name="checkStrings"/> is "female" after normalization, returns <see cref="GenderAlignment.Female"/></para>
	/// </summary>
	/// <param name="checkStrings">The string to examine.</param>
	/// <returns><see cref="GenderAlignment.Male"/> if <paramref name="checkStrings"/> has "male", <see cref="GenderAlignment.Female"/> if <paramref name="checkStrings"/> has "female", otherwise <see cref="GenderAlignment.None"/></returns>
	/// <exception cref="Exception">Throws if two different gender strings were found in the enumerable.</exception>
	public static GenderAlignment HasGenderName(IEnumerable<string> checkStrings)
	{


		GenderAlignment seen = GenderAlignment.None;

		foreach (string str in checkStrings)
		{

			GenderAlignment result = IsGenderName(str);

			if (result == GenderAlignment.None)
				continue;

			if (seen == GenderAlignment.None)
				seen = result;

			if (result != seen)
				throw new Exception($"Two different gender strings were found in the enumerable: '{seen}' found first, '{result}' found later.");

		}

		return seen;

	}

	/// <summary>
	/// Enumerates all <paramref name="potentialForms"/> that <paramref name="baseForm"/> could link to.
	/// </summary>
	/// <param name="baseForm"><see cref="MetaForm"/> to attempt to link to <paramref name="potentialForms"/>.</param>
	/// <param name="potentialForms">Enumerable of <see cref="MetaForm"/> that <paramref name="baseForm"/> will attempt to link to.</param>
	/// <param name="genderAlignment">The <see cref="GenderAlignment"/> to use for <paramref name="baseForm"/>.</param>
	/// <returns>An enumerable with all <paramref name="potentialForms"/> that <paramref name="baseForm"/> was able to link to.</returns>
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
