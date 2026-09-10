using GraphForge.Api.Database;
using GraphForge.Api.DTOs.Auth;
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
        await _authService.SignInAsync(request);
        return NoContent();
    }


    [HttpPost("signup")]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        await _authService.SignUpAsync(request);
        return NoContent();
    }

    [HttpPost("signout")]
    public async Task<IActionResult> LogOut()
    {
        await _authService.LogOutAsync();

        return NoContent();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        await _authService.RefreshTokenAsync();

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        CurrentUserInfoResponse result = await _authService.Me();

        return Ok(result);
    }
}
