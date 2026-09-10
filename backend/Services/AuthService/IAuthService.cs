using GraphForge.Api.DTOs.Auth;
namespace GraphForge.Api.Services.AuthService
{
    public interface IAuthService
    {
        Task SignInAsync(SignInRequest request);
        Task SignUpAsync(SignUpRequest request);
        Task LogOutAsync();
        Task RefreshTokenAsync();
        Task<CurrentUserInfoResponse> Me();
    }
}
