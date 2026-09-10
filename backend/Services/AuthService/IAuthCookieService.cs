namespace GraphForge.Api.Services.AuthService;

public interface IAuthCookieService
{
    void SetAccessToken(string token);
    void SetRefreshToken(string token);
    string? GetRefreshToken();
    void ClearAuthCookies();
}
