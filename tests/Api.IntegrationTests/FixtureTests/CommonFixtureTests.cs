using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.FixtureTests;

public class CommonFixtureTests : TestBase
{
    public CommonFixtureTests(CommonTestFixture fixture) : base(fixture) { }

    [Fact]
    public void TestCollection_ShouldInitialize()
    {
        Assert.True(true);
    }
}