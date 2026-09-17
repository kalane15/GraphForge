using GraphForge.Api.Models;

namespace GraphForge.Api.Services.AuthService;

public interface ISessionService
{
    Task<string> CreateSessionAsync(User user);
    Task<Session> GetValidSessionByRefreshTokenAsync(string refreshToken);
    Task DeleteSessionByRefreshTokenAsync(string refreshToken);
}
