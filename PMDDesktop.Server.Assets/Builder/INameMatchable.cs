namespace PMDDesktop.Server.Assets.Builder;

internal interface INameMatchable
{

	internal static int CalculateNameMatches(INameMatchable left, INameMatchable right)
	{

		string[] leftStrings = [.. left.GetMatchableParts()];
		string[] rightStrings = [.. right.GetMatchableParts()];

		return CompareNames(leftStrings, rightStrings);

	}

	private static int CompareNames(string[] left, string[] right)
	{

		int matches = 0;

		for (int leftStartIndex = 0; leftStartIndex < left.Length; ++leftStartIndex)
			for (int leftEndIndex = leftStartIndex + 1; leftEndIndex <= left.Length; ++leftEndIndex)
				for (int rightStartIndex = 0; rightStartIndex < right.Length; ++rightStartIndex)
					for (int rightEndIndex = rightStartIndex + 1; rightEndIndex <= right.Length; ++rightEndIndex)
					{

						string leftSection = NormalizeStringForMatching(string.Join("", left[leftStartIndex..leftEndIndex]));
						string rightSection = NormalizeStringForMatching(string.Join("", right[rightStartIndex..rightEndIndex]));

						if (leftSection == rightSection)
							++matches;

					}

		return matches;

	}

	static string NormalizeStringForMatching(string inputString)
	{

		inputString = inputString.ToLowerInvariant();

		inputString = new([.. inputString.Where(char.IsAsciiLetterLower)]);

		return inputString;

	}

	static IEnumerable<string> NormalizeStringsForMatching(IEnumerable<string> inputStrings)
	{

		foreach (string inputString in inputStrings)
			yield return NormalizeStringForMatching(inputString);

		yield break;

	}

	IEnumerable<string> GetMatchableParts();

}
