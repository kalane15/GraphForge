using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GraphForge.Api.Database;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        string backendPath = ResolveBackendPath();
        string rootPath = Directory.GetParent(backendPath)?.FullName ?? backendPath;
        Dictionary<string, string> dotenv = ReadDotenv(Path.Combine(rootPath, ".env"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(backendPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? BuildConnectionString(dotenv);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new AppDbContext(options);
    }

    private static string ResolveBackendPath()
    {
        string currentPath = Directory.GetCurrentDirectory();

        if (File.Exists(Path.Combine(currentPath, "appsettings.json")))
        {
            return currentPath;
        }

        string backendPath = Path.Combine(currentPath, "backend");

        if (File.Exists(Path.Combine(backendPath, "appsettings.json")))
        {
            return backendPath;
        }

        return currentPath;
    }

    private static Dictionary<string, string> ReadDotenv(string path)
    {
        if (!File.Exists(path))
        {
            return new Dictionary<string, string>();
        }

        var values = new Dictionary<string, string>();

        foreach (string line in File.ReadLines(path))
        {
            string trimmedLine = line.Trim();

            if (trimmedLine.Length == 0 || trimmedLine.StartsWith('#'))
            {
                continue;
            }

            string[] parts = trimmedLine.Split('=', 2);

            if (parts.Length != 2)
            {
                continue;
            }

            values[parts[0]] = parts[1];
        }

        return values;
    }

    private static string BuildConnectionString(IReadOnlyDictionary<string, string> dotenv)
    {
        string database = GetConfigurationValue("POSTGRES_DB", dotenv) ?? "graphforge";
        string username = GetConfigurationValue("POSTGRES_USER", dotenv) ?? "graphforge";
        string password = GetConfigurationValue("POSTGRES_PASSWORD", dotenv) ?? "some_password";

        return $"Host=localhost;Port=5432;Database={database};Username={username};Password={password}";
    }

    private static string? GetConfigurationValue(string key, IReadOnlyDictionary<string, string> dotenv)
    {
        return Environment.GetEnvironmentVariable(key)
               ?? (dotenv.TryGetValue(key, out string? value) ? value : null);
    }
}
