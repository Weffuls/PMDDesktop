namespace PMDDesktop.Server.Assets.Builder.Species;

internal class NameMatchableGroup : INameMatchable
{

	internal NameMatchableGroup(IEnumerable<string> names)
	{

		this.names = names;

	}

	private IEnumerable<string> names;

	public IEnumerable<string> GetMatchableParts() => names;

}
