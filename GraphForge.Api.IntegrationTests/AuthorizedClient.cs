using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace GraphForge.Api.IntegrationTests;

public static class AuthTestClientExtensions
{
    public static async Task<HttpClient> CreateAuthorizedClientAsync(
        this ApiPostgresTestFactory factory, string? clientLogin=null)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        string login = clientLogin ?? Guid.NewGuid().ToString();
        string password = "123";

        var response = await client.PostAsJsonAsync("/api/auth/signup", new
        {
            login,
            password
        });

        response.EnsureSuccessStatusCode();

        return client;
    }
}
