using GraphForge.Api;
using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.Models;
using GraphForge.Api.Services.AuthService;
using GraphForge.Api.Services.GraphService;
using GraphForge.Api.Services.ProjectService;
using GraphForge.Api.Services.SchemasService;
using GraphForge.Api.Services.UserIdentityProviderService;
using GraphForge.Validation;
using GraphForge.Validation.GraphValidationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
AddAuth(builder);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        string frontendUrl = builder.Configuration["FRONTEND_URL"]
            ?? throw new InvalidOperationException(
                "FRONTEND_URL is not configured"
            );

        policy
            .WithOrigins(frontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention()
);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .SelectMany(entry => entry.Value!.Errors.Select(error =>
                $"{entry.Key}: {error.ErrorMessage}"
            ));

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation error",
            Detail = string.Join("; ", errors)
        };

        return new BadRequestObjectResult(problem);
    };
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IAuthCookieService, AuthCookieService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProjectsService, ProjectsService>();
builder.Services.AddScoped<IGraphsService, GraphsService>();
builder.Services.AddScoped<IGraphJsonValidatorService, GraphJsonValidatorService>();
builder.Services.AddScoped<ISchemaDtoValidatorService, SchemaDtoValidatorService>();
builder.Services.AddScoped<ISchemasService, SchemasService>();
builder.Services.AddScoped<IUserIdentityProvider, UserIdentityProvider>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(ExceptionHandler.Handler);
});


app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void AddAuth(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton(serviceProvider =>
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

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer();

    builder.Services
        .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
        .Configure<AuthOptions>((options, authOptions) =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = authOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = authOptions.Audience,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(authOptions.Key)
                )
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["access_token"];
                    return Task.CompletedTask;
                }
            };
        });

    builder.Services.AddAuthorization();
}

public partial class Program
{
}
