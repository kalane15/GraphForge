using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GraphForge.Api.IntegrationTests;

public static class AuthTestClientExtensions
{
    /// <summary>
    /// Creates new client with passed authorization. New client is created every call
    /// </summary>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static async Task<HttpClient> CreateAuthorizedClientAsync(
        this ApiPostgresTestFactory factory)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        string login = Guid.NewGuid().ToString();
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
