using PMDDesktop.Server.Assets.Data;

namespace PMDDesktop.Server.Assets.Tests.Builder;

public class BuildSpeciesTests(BuildSpeciesFixture fixture) : IClassFixture<BuildSpeciesFixture>
{

	private BuildSpeciesFixture fixture = fixture;

	[Fact]
	public void CanReadMockZip()
	{

		Assert.NotNull(fixture.apiZip);
		Assert.NotNull(fixture.spriteZip);

	}

	[Fact]
	public void ContainsAtLeastOneSpecies()
	{
		Assert.NotEmpty(fixture.assets.OfType<Species>());
	}

	[Fact]
	public void ContainsAtLeastOneSpeciesVariant()
	{
		Assert.NotEmpty(fixture.assets.OfType<Species>());
	}

	[Fact]
	public void ContainsAtLeastOneSpeciesForm()
	{
		Assert.NotEmpty(fixture.assets.OfType<Species>());
	}

}
