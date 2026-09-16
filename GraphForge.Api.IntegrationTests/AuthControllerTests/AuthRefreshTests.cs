using GraphForge.Api.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace GraphForge.Api.IntegrationTests.AuthTests;

public class AuthRefreshTests : IClassFixture<ApiPostgresTestFactory>
{
    private readonly HttpClient _client;

    public AuthRefreshTests(ApiPostgresTestFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }


    [Fact]
    public async Task Refresh_WhenSignedIn_ReturnsNoContentAndKeepsUserAuthorized()
    {
        string login = Guid.NewGuid().ToString("N");
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


        response = await _client.PostAsync("/api/auth/refresh", content: null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);


        response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        CurrentUserInfoResponse? info = await response.Content.ReadFromJsonAsync<CurrentUserInfoResponse>();
        Assert.NotNull(info);
        Assert.Equal(login, info.Login);
    }


    [Fact]
    public async Task Refresh_WhenNotSignedIn_Returns401ProblemDetails()
    {
        var response = await _client.PostAsync("/api/auth/refresh", content: null);

        await AssertProblemDetailsAsync(response, HttpStatusCode.Unauthorized);
    }
}
