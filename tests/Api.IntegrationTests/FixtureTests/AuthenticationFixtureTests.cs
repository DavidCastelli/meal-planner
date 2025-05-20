using Api.IntegrationTests.Infrastructure;
using Api.IntegrationTests.Infrastructure.Fixtures;

namespace Api.IntegrationTests.FixtureTests;

public class AuthenticationFixtureTests : TestBaseAuthentication
{
    public AuthenticationFixtureTests(AuthenticationTestFixture fixture) : base(fixture) { }

    [Fact]
    public void AuthenticationTestCollection_ShouldInitialize()
    {
        Assert.True(true);
    }
}