using GraphForge.Api.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace GraphForge.Api.IntegrationTests;

public sealed class ApiPostgresTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:18")
        .WithDatabase("graphforge_tests")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private string? _connectionString;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _connectionString = _postgres.GetConnectionString();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();
    }

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
                ["ConnectionStrings:DefaultConnection"] = _connectionString
            });
        });

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _postgres.DisposeAsync();

        Dispose();
    }
}
