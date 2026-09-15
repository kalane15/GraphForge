using GraphForge.Api.DTOs.Auth;
using System.Net;
using System.Net.Http.Json;

namespace GraphForge.Api.IntegrationTests.AuthTests;

public class AuthSignInSignUpTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly HttpClient _client;

    public AuthSignInSignUpTests(ApiPostgresTestFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SignInSignUp_WhenSignUpThenSignIn_Returns204()
    {
        string login = "123";
        string password = "123";

        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            login,
            password
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await _client.PostAsJsonAsync("/api/auth/signin", new
        {
            login,
            password
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task SignIn_WhenPasswordIncorrect_Returns401()
    {
        string login = "incorrectPasswordUser";
        string password = "123";

        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            login,
            password
        });


        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await _client.PostAsJsonAsync("/api/auth/signin", new
        {
            login,
            password="some trash"
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task SignUp_WhenUserWithLoginExists_Returns409()
    {
        string login = "duplicateUser";
        string password = "123";

        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            login,
            password
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            login,
            password="1234"
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task SignIn_WhenUserDoesNotExists_Returns401()
    {
        string login = "dontExistUser";
        string password = "123";

        var response = await _client.PostAsJsonAsync("/api/auth/signin", new
        {
            login,
            password
        });

        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_WhenSignIn_ReturnsSameLogin()
    {
        string login = "meUser";
        string password = "123";

        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            login,
            password
        });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await _client.PostAsJsonAsync("/api/auth/signin", new
        {
            login,
            password
        });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        response = await _client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        CurrentUserInfoResponse? info = await response.Content.ReadFromJsonAsync<CurrentUserInfoResponse>();
        Assert.NotNull(info);
        Assert.Equal(login, info.Login);
    }
}
