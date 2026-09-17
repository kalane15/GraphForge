using GraphForge.Api.Models;

namespace GraphForge.Api.Services.AuthService;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);
}
