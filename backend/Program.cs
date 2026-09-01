using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();


string frontendUrl = builder.Configuration["FRONTEND_URL"]
    ?? throw new InvalidOperationException(
        "FRONTEND_URL is not configured"
    );


var authOptions = new AuthOptions
{
    Issuer = builder.Configuration["AUTH_ISSUER"]
        ?? throw new InvalidOperationException("AUTH_ISSUER is not configured"),

    Audience = builder.Configuration["AUTH_AUDIENCE"]
        ?? throw new InvalidOperationException("AUTH_AUDIENCE is not configured"),

    Key = builder.Configuration["JWT_KEY"]
        ?? throw new InvalidOperationException("JWT_KEY is not configured"),
};

builder.Services.AddSingleton(authOptions);

var keyBytes = Encoding.UTF8.GetBytes(authOptions.Key);

if (keyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "JWT_KEY must be at least 32 bytes long."
    );
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
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

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
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
                context.Token =
                    context.Request.Cookies["access_token"];

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();
app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
