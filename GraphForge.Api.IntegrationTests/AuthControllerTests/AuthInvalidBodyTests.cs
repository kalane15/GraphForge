using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api.IntegrationTests.AuthTests.AuthTests;

public sealed class AuthInvalidBodyTests : IClassFixture<ApiTestFactory>
{
    private readonly HttpClient _client;

    public AuthInvalidBodyTests(ApiTestFactory factory)
    {
        _client = factory.CreateClient();
    }
    public static IEnumerable<object[]> InvalidBodies =>
    [
        [
            new
            {
                login = "test"
            }
        ],
        [
            new
            {
                login = "test",
                password = ""
            }
        ]
    ];

    [Theory]
    [MemberData(nameof(InvalidBodies))]
    public async Task SignIn_WhenPasswordIsMissingOrEmpty_ReturnsProblemDetails(object body)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/signin", body);

        ProblemDetails problem = await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal("Validation error", problem.Title);
        Assert.Contains("Password", problem.Detail, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [MemberData(nameof(InvalidBodies))]
    public async Task SignUp_WhenPasswordIsEmpty_Returns400ProblemDetails(object body)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/signup", body);

        ProblemDetails problem = await AssertProblemDetailsAsync(response, HttpStatusCode.BadRequest);
        Assert.Equal("Validation error", problem.Title);
        Assert.Contains("Password", problem.Detail, StringComparison.OrdinalIgnoreCase);
    }
}
