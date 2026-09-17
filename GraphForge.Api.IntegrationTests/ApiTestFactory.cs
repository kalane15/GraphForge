using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GraphForge.Api.IntegrationTests;

public sealed class ApiTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["FRONTEND_URL"] = "http://localhost:5173",
                ["AUTH_ISSUER"] = "GraphForge.Tests",
                ["AUTH_AUDIENCE"] = "GraphForge.Tests",
                ["JWT_KEY"] = "test-jwt-key-with-at-least-32-bytes",
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=graphforge_tests;Username=test;Password=test"
            });
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
    }
}
