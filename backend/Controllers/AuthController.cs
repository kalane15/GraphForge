using GraphForge.Api.DTOs.Auth;
using GraphForge.Api.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GraphForge.Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
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
