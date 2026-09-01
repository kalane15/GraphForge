using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GraphForge.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private const int AccessTokenExpirationTimeMinutes = 15;
    private const int RefreshTokenExpirationTimeMinutes = 30 * 24 * 60;

    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly AuthOptions _authOptions;

    public AuthController(
       AppDbContext db,
       IPasswordHasher<User> passwordHasher,
       AuthOptions authOptions)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _authOptions = authOptions;
    }


    [HttpPost("signin")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        User? user = await _db.Users
            .FirstOrDefaultAsync(u => u.Login == request.Login);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "User not found"
            });
        }

        PasswordVerificationResult verifyPasswordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        bool isCorrectPassword = verifyPasswordResult != PasswordVerificationResult.Failed;

        if (!isCorrectPassword)
        {
            return Unauthorized(new
            {
                message = "Incorrect password"
            });
        }

        ProvideAccessToken(request.Login);
        await CreateSession(user.Id);

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Signed in successfully"
        });
    }


    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        bool userExists = await _db.Users
            .AnyAsync(user => user.Login == request.Login);

        if (userExists)
        {
            return Conflict(new
            {
                message = "User with this login already exists"
            });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);

        ProvideAccessToken(request.Login);
        await CreateSession(user.Id);

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Signed up successfully"
        });
    }

    [HttpPost("signout")]
    public IActionResult LogOut()
    {
        Response.Cookies.Delete("access_token");

        return Ok();
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh()
    {
        string? refreshToken = Request.Cookies["refresh_token"];

        if (refreshToken is null)
        {
            return Unauthorized();
        }

        string hash = HashRefreshToken(refreshToken);

        Session? session = await _db.Sessions
            .FirstOrDefaultAsync(s =>
                s.RefreshTokenHash == hash &&
                s.ExpiresAt > DateTimeOffset.UtcNow);

        if (session is null)
        {
            return Unauthorized();
        }

        User user = await _db.Users.SingleAsync((u) => u.Id == session.UserId);

        ProvideAccessToken(user.Login);

        return Ok();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        if (User.Identity?.Name == null)
        {
            return Unauthorized(new
            {
                message = "Unauthorized"
            });
        }

        return Ok(new
        {
            login = User.Identity?.Name
        });
    }

    private void ProvideAccessToken(string login)
    {

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, login),
            new Claim(ClaimTypes.Role, "user"),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_authOptions.Key)
        );

        JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AccessTokenExpirationTimeMinutes)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpirationTimeMinutes)
        });
    }

    private async Task CreateSession(Guid userId)
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(64);

        string token = Convert.ToBase64String(bytes);

        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(RefreshTokenExpirationTimeMinutes),
            RefreshTokenHash = HashRefreshToken(token)
        };

        _db.Sessions.Add(session);

        await _db.SaveChangesAsync();

        Response.Cookies.Append(
            "refresh_token",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });
    }

    private string HashRefreshToken(string token)
    {
        byte[] tokenHash = SHA256.HashData(
           Encoding.UTF8.GetBytes(token)
       );
        string hashedToken = Convert.ToHexString(tokenHash);
        return hashedToken;
    }
}
