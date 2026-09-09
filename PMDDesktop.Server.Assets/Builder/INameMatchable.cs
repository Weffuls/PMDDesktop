namespace PMDDesktop.Server.Assets.Builder;

internal interface INameMatchable
{

	internal static int CalculateNameMatches(INameMatchable left, INameMatchable right)
	{

		string[] leftStrings = [.. left.GetMatchableParts()];
		string[] rightStrings = [.. right.GetMatchableParts()];

		return CompareNames(leftStrings, rightStrings);

	}

	private static int CompareNames(string[] longer, string[] shorter)
	{

		int matches = 0;

		for (int leftStartIndex = 0; leftStartIndex < longer.Length; ++leftStartIndex)
			for (int leftEndIndex = leftStartIndex + 1; leftEndIndex <= longer.Length; ++leftEndIndex)
				for (int rightStartIndex = 0; rightStartIndex < shorter.Length; ++rightStartIndex)
					for (int rightEndIndex = rightStartIndex + 1; rightEndIndex <= shorter.Length; ++rightEndIndex)
					{

						string shortString = NormalizeStringForMatching(string.Join("", longer[leftStartIndex..leftEndIndex]));
						string longString = NormalizeStringForMatching(string.Join("", longer[leftStartIndex..leftEndIndex]));

						if (shortString == longString)
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
