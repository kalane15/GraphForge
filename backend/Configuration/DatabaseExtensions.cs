using GraphForge.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Configuration;

internal static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention()
        );

        return services;
    }
}
