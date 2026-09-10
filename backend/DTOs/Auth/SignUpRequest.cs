using System.ComponentModel.DataAnnotations;

namespace GraphForge.Api.DTOs.Auth;


public record SignUpRequest(
    [Required] string Login,
    [Required] string Password
);
