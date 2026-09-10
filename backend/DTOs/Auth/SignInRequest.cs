using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Auth;


public record SignInRequest(
    [Required] string Login,
    [Required] string Password
);
