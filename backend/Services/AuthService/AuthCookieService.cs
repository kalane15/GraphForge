namespace GraphForge.Api.Services.AuthService;

public class AuthCookieService : IAuthCookieService
{
    private const int AccessTokenExpirationTimeMinutes = 15;
    private const int RefreshTokenExpirationTimeMinutes = 30 * 24 * 60;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthCookieService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetAccessToken(string token)
    {
        HttpResponse response = _httpContextAccessor.HttpContext!.Response;

        response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpirationTimeMinutes)
        });
    }

    public void SetRefreshToken(string token)
    {
        HttpResponse response = _httpContextAccessor.HttpContext!.Response;

        response.Cookies.Append("refresh_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddMinutes(RefreshTokenExpirationTimeMinutes)
        });
    }

    public string? GetRefreshToken()
    {
        HttpRequest request = _httpContextAccessor.HttpContext!.Request;
        return request.Cookies["refresh_token"];
    }

    public void ClearAuthCookies()
    {
        HttpResponse response = _httpContextAccessor.HttpContext!.Response;

        response.Cookies.Delete("access_token");
        response.Cookies.Delete("refresh_token");
    }
}
