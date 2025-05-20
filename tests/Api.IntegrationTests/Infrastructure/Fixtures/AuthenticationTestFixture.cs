namespace Api.IntegrationTests.Infrastructure.Fixtures;

public sealed class AuthenticationTestFixture : CommonTestFixture
{
    public string AuthCookie { get; private set; } = null!;

    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();
        await InitializeAuthCookieAsync();
    }

    private async Task InitializeAuthCookieAsync()
    {
        var httpClient = CreateClient();

        var email = TestDataSeeder.GetTestUser().Email;
        var password = TestDataSeeder.GetTestUserPassword();
        var response = await httpClient.PostAsJsonAsync("api/login?useCookies=true", new { email, password });

        AuthCookie = response.Headers.GetValues("Set-Cookie").SingleOrDefault()
                     ?? throw new InvalidOperationException("Failed to perform necessary login request to retrieve authentication cookie.");
    }
}