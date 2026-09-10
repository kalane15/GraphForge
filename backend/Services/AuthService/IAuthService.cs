using GraphForge.Api.DTOs.Auth;
using GraphForge.Api.Models;

namespace GraphForge.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task ProvideAccessTokenAsync(User user);
        Task ProvideSessionAsync(User user);
        Task EndCurrentSessionAsync();
        Task RefreshAccessTokenAsync();
        Task SignInAsync(SignInRequest request);
        Task SignUpAsync(SignUpRequest request);
        Task LogOutAsync();
        Task RefreshTokenAsync();
        Task<CurrentUserInfoResponse> Me();
    }
}
