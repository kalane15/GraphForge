using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using GraphForge.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GraphForge.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IAuthService _authService;

    public AuthController(
       AppDbContext db,
       IPasswordHasher<User> passwordHasher,
       IAuthService authService)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _authService = authService;
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

        await _authService.ProvideAccessTokenAsync(user);
        await _authService.ProvideSessionAsync(user);

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
        await _db.SaveChangesAsync();

        await _authService.ProvideAccessTokenAsync(user);
        await _authService.ProvideSessionAsync(user);

        return Ok(new
        {
            message = "Signed up successfully"
        });
    }

    [HttpPost("signout")]
    public async Task<IActionResult> LogOut()
    {
        await _authService.EndCurrentSessionAsync();

        return Ok();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        bool refreshed = await _authService.RefreshAccessTokenAsync();

        return refreshed ? Ok() : Unauthorized();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (User.Identity?.Name == null)
        {
            return Unauthorized(new
            {
                message = "Unauthorized"
            });
        }

        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return StatusCode(500, new
            {
                message = "User claim does not exist"
            });
        }

        bool userExists = await _db.Users.AnyAsync(u => u.Id == userId);

        if (!userExists) { 
            return Unauthorized(new
            {
                message = "User not found"
            });
        }
        

        return Ok(new
        {
            login = User.Identity?.Name
        });
    }
}
