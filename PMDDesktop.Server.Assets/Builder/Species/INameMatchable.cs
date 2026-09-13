namespace PMDDesktop.Server.Assets.Builder.Species;

/// <summary>
/// Interface for matching names inside the <see cref="BuildSpecies"/> routine, both strictly and loosely.
/// </summary>
internal interface INameMatchable
{

	/// <summary>
	/// <para>A collection of lists of words that can be matched, even if their content is strictly different.</para>
	/// <para>For example, different words that mean the same thing.</para>
	/// <para>Yes this is a hard-coded collection of overrides, yes better solutions would be loved.</para>
	/// </summary>
	internal static readonly string[][] SYNONYM_GROUPS = [
		[
			"gmax",
			"gigantamax"
		],
		[
			"savannah",
			"savanna"
		]
	];

	/// <summary>
	/// Tries a bunch of different methods to match names. Higher numbers mean they're closer.
	/// </summary>
	/// <param name="left">Left-side <see cref="INameMatchable"/></param>
	/// <param name="right">Right-side <see cref="INameMatchable"/></param>
	/// <returns>A number indicating how similar the names are.</returns>
	internal static int CalculateNameMatches(INameMatchable left, INameMatchable right)
	{

		string[] leftStrings = [.. left.GetMatchableParts()];
		string[] rightStrings = [.. right.GetMatchableParts()];

		return CompareNames(leftStrings, rightStrings);

	}

	/// <summary>
	/// Sees if both <see cref="INameMatchable"/>s have the same words.
	/// </summary>
	/// <param name="left">Left-side <see cref="INameMatchable"/></param>
	/// <param name="right">Right-side <see cref="INameMatchable"/></param>
	/// <returns>True if both names contain all the same words.</returns>
	internal static bool IsSameNameAnyOrder(INameMatchable left, INameMatchable right)
	{

		List<string> leftNames = [.. left.GetMatchableParts().Select(NormalizeStringForMatching)];
		List<string> rightNames = [.. right.GetMatchableParts().Select(NormalizeStringForMatching)];

		if (leftNames.Count != rightNames.Count)
			return false;

		foreach (string leftName in leftNames)
			if (!rightNames.Remove(leftName)) // TODO: Let this use SinglePartsMatch somehow.
				return false;

		return true;

	}

	/// <summary>
	/// Sees if both <see cref="INameMatchable"/>s have the same words, in the exact same order.
	/// </summary>
	/// <param name="left">Left-side <see cref="INameMatchable"/></param>
	/// <param name="right">Right-side <see cref="INameMatchable"/></param>
	/// <returns>True if both names contain all the same words.</returns>
	/// <remarks>Honestly this is just here to further clarify the purpose of <see cref="IsSameNameAnyOrder"/></remarks>
	internal static bool IsSameNameExact(INameMatchable left, INameMatchable right)
	{

		string[] leftNames = [.. left.GetMatchableParts()];
		string[] rightNames = [.. right.GetMatchableParts()];

		if (leftNames.Length != rightNames.Length)
			return false;

		for (int nameIndex = 0; nameIndex < leftNames.Length; ++nameIndex)
			if (!SinglePartsMatch(leftNames[nameIndex], rightNames[nameIndex]))
				return false;

		return true;

	}

	/// <summary>
	/// Tries to shift and combine words to match names. Higher numbers mean they're closer.
	/// </summary>
	/// <param name="left">Left-side array of strings.</param>
	/// <param name="right">Right-side array of strings.</param>
	/// <returns>A number indicating how similar the names are.</returns>
	private static int CompareNames(string[] left, string[] right)
	{

		int matches = 0;

		for (int leftStartIndex = 0; leftStartIndex < left.Length; ++leftStartIndex)
			for (int leftEndIndex = leftStartIndex + 1; leftEndIndex <= left.Length; ++leftEndIndex)
				for (int rightStartIndex = 0; rightStartIndex < right.Length; ++rightStartIndex)
					for (int rightEndIndex = rightStartIndex + 1; rightEndIndex <= right.Length; ++rightEndIndex)
					{

						string leftSection = string.Join("", left[leftStartIndex..leftEndIndex]);
						string rightSection = string.Join("", right[rightStartIndex..rightEndIndex]);

						if (SinglePartsMatch(leftSection, rightSection))
							++matches;

					}

		return matches;

	}

	/// <summary>
	/// <para>Do these two strings match according to our matching rules?</para>
	/// <para>Matching rules include a direct string comparison after 'normalizing' the strings, and a check if they share a group in <see cref="SYNONYM_GROUPS"/>.</para>
	/// </summary>
	/// <param name="left">Left-side string.</param>
	/// <param name="right">Right-side string.</param>
	/// <returns>True if they do match.</returns>
	static bool SinglePartsMatch(string left, string right)
	{

		// Safety normalization.
		left = NormalizeStringForMatching(left);
		right = NormalizeStringForMatching(right);

		// Quick and easy!!
		if (left == right)
			return true;

		foreach (string[] group in SYNONYM_GROUPS)
		{

			if (group.Contains(left) && group.Contains(right))
				return true;

		}

		return false;

	}

	/// <summary>
	/// <para>Applies a few rules to make <paramref name="inputString"/> more suitable for matching.</para>
	/// <para>Modifications include but are not limited to: going all-lowercase, removing all characters except a-z or 0-9, and trimming whitespace.</para>
	/// </summary>
	/// <param name="inputString">String to normalize.</param>
	/// <returns>Returns the new, modified string.</returns>
	static string NormalizeStringForMatching(string inputString)
	{

		if (string.IsNullOrWhiteSpace(inputString))
			throw new ArgumentException($"Input string was null or whitespace: '{inputString}'", nameof(inputString));

		string normalizedString = inputString.ToLowerInvariant().Trim();

		// Remove all characters except AsciiLetterLower !!UNLESS!! there is ONLY digits in the string.
		// This is required, as Zygarde has a form named '10'.
		if (!normalizedString.All(char.IsAsciiDigit))
			normalizedString = new([.. normalizedString.Where(char.IsAsciiLetterLower)]);

		if (string.IsNullOrWhiteSpace(normalizedString))
			throw new ArgumentException($"Input string had no comparable characters left after normalizing: '{inputString}' (either only ascii letters or only numbers are allowed)", nameof(inputString));

		return normalizedString;

	}

	/// <summary>
	/// <para>Applies rules to every string in <paramref name="inputStrings"/> to make them more suitable for matching.</para>
	/// <para>Modifications include but are not limited to: going all-lowercase, removing all characters except a-z or 0-9, and trimming whitespace.</para>
	/// </summary>
	/// <param name="inputStrings">Strings to normalize.</param>
	/// <returns>Returns an enumerable with the modified strings.</returns>
	/// <seealso cref="NormalizeStringForMatching(string)"/>
	static IEnumerable<string> NormalizeStringsForMatching(IEnumerable<string> inputStrings)
	{

		foreach (string inputString in inputStrings)
			yield return NormalizeStringForMatching(inputString);

		yield break;

	}

	internal static IEnumerable<string> RemoveCommonNames(INameMatchable primary, IEnumerable<INameMatchable> matchables)
	{

		List<string> uncommonNames = [];

		IEnumerable<INameMatchable> noSelf = matchables.Where(matchable => matchable != primary);

		int count = noSelf.Count();

		foreach (string primaryName in primary.GetMatchableParts())
		{

			IEnumerable<IEnumerable<string>> normalizedNames = noSelf.Select(match => NormalizeStringsForMatching(match.GetMatchableParts()));

			if (normalizedNames.Count(names => names.Contains(primaryName)) != count)
				uncommonNames.Add(primaryName);

		}

		return uncommonNames;

	}

	/// <summary>
	/// Get all individual "words" for matching with <see cref="INameMatchable"/>.
	/// </summary>
	/// <returns>An enumerable containing "words" to use in matching.</returns>
	IEnumerable<string> GetMatchableParts();

}
