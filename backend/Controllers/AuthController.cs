using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphForge.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly AuthOptions _authOptions;

    public AuthController(
       AppDbContext db,
       IPasswordHasher<User> passwordHasher,
       IOptions<AuthOptions> authOptions)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _authOptions = authOptions.Value;
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

        var claims = new List<Claim> { 
            new Claim("login", request.Login),
            new Claim("role", "user"),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_authOptions.Key)
        );

        JwtSecurityToken jwtToken = new JwtSecurityToken(
                issuer: _authOptions.Issuer,
                audience: _authOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
        });
    }


    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        bool userExists = await _db.Users
            .AnyAsync(user => user.Login == request.Login);

        if (userExists)
            return Conflict("User with this login already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            CreatedAt = DateTimeOffset.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        return Ok();
    }
}
