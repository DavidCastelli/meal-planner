using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.FixtureTests;

public class AuthorizationFixtureTests : TestBaseAuthorization
{
    public AuthorizationFixtureTests(AuthorizationTestFixture fixture) : base(fixture) { }

    [Fact]
    public void AuthorizationTestCollection_ShouldInitialize()
    {
        Assert.True(true);
    }
}