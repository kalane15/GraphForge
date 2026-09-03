using GraphForge.Api.Database;
using GraphForge.Api.DTOs;
using GraphForge.Api.Models;
using GraphForge.Api.Services.AuthService;
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

    private static ProblemDetails UnauthorizedDetails(string detail) => new()
    {
        Status = StatusCodes.Status401Unauthorized,
        Title = "Unauthorized",
        Detail = detail
    };

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
            return Unauthorized(UnauthorizedDetails("User does not exist"));
        }

        PasswordVerificationResult verifyPasswordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        bool isCorrectPassword = verifyPasswordResult != PasswordVerificationResult.Failed;

        if (!isCorrectPassword)
        {
            return Unauthorized(UnauthorizedDetails("Incorrect password"));
        }

        await _authService.ProvideAccessTokenAsync(user);
        await _authService.ProvideSessionAsync(user);

        return NoContent();
    }


    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        bool userExists = await _db.Users
            .AnyAsync(user => user.Login == request.Login);

        if (userExists)
        {
            return Conflict(new ProblemDetails()
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "User already exists"
                }
            );
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

        return NoContent();
    }

    [HttpPost("signout")]
    public async Task<IActionResult> LogOut()
    {
        await _authService.EndCurrentSessionAsync();

        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        bool refreshed = await _authService.RefreshAccessTokenAsync();

        return refreshed ? NoContent() : Unauthorized(UnauthorizedDetails("User refresh failed"));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (User.Identity?.Name == null)
        {
            return Unauthorized(UnauthorizedDetails("Authorization is required"));
        }

        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return StatusCode(500, new ProblemDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = "Unable to get user claim"
            });
        }

        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) { 
            return Unauthorized(UnauthorizedDetails("User not found"));
        }
        

        return Ok(new CurrentUserInfoResponse(user.Login, user.CreatedAt));
    }
}
