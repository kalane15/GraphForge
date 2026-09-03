using GraphForge.Api.Models;

namespace GraphForge.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task ProvideAccessTokenAsync(User user);
        Task ProvideSessionAsync(User user);
        Task EndCurrentSessionAsync();
        Task<bool> RefreshAccessTokenAsync();
    }
}
