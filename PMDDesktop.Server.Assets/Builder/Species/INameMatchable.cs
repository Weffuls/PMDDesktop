namespace PMDDesktop.Server.Assets.Builder.Species;

internal interface INameMatchable
{

	/// <summary>
	/// A collection of lists of words that can be matched, even if their content is strictly different.
	/// For example, different words that mean the same thing.
	/// Yes this is a hard-coded collection of overrides, yes better solutions would be loved.
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

	internal static int CalculateNameMatches(INameMatchable left, INameMatchable right)
	{

		string[] leftStrings = [.. left.GetMatchableParts()];
		string[] rightStrings = [.. right.GetMatchableParts()];

		return CompareNames(leftStrings, rightStrings);

	}

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

	static IEnumerable<string> NormalizeStringsForMatching(IEnumerable<string> inputStrings)
	{

		foreach (string inputString in inputStrings)
			yield return NormalizeStringForMatching(inputString);

		yield break;

	}

	IEnumerable<string> GetMatchableParts();

}
