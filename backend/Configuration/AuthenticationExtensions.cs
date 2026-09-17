using GraphForge.Api.Auth;
using GraphForge.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace GraphForge.Api.Configuration;

internal static class AuthenticationExtensions
{
    public static IServiceCollection AddGraphForgeAuthentication(
        this IServiceCollection services)
    {
        AddAuthOptions(services);
        services.AddHttpContextAccessor();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<AuthOptions>((options, authOptions) =>
            {
                options.TokenValidationParameters = JwtBearerConfiguration.CreateTokenValidationParameters(authOptions);

                options.Events = JwtBearerConfiguration.CreateJwtBearerEvents();
            });

        services.AddAuthorization();

        return services;
    }

    private static void AddAuthOptions(IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var authOptions = new AuthOptions
            {
                Issuer = configuration["AUTH_ISSUER"]
                    ?? throw new InvalidOperationException("AUTH_ISSUER is not configured"),

                Audience = configuration["AUTH_AUDIENCE"]
                    ?? throw new InvalidOperationException("AUTH_AUDIENCE is not configured"),

                Key = configuration["JWT_KEY"]
                    ?? throw new InvalidOperationException("JWT_KEY is not configured"),
            };

            var keyBytes = Encoding.UTF8.GetBytes(authOptions.Key);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "JWT_KEY must be at least 32 bytes long."
                );
            }

            return authOptions;
        });
    }
}
