using GraphForge.Api.Database;
using GraphForge.Api.DTOs.Auth;
using GraphForge.Api.Models;
using GraphForge.Api.Services.UserIdentityProviderService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GraphForge.Api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserIdentityProvider _userIdentityProvider;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ISessionService _sessionService;
        private readonly IAuthCookieService _authCookieService;


        public AuthService(
            AppDbContext db,
            IPasswordHasher<User> hasher,
            IUserIdentityProvider userIdentityProvider,
            IJwtTokenService jwtTokenService,
            ISessionService sessionService,
            IAuthCookieService authCookieService)
        {
            _db = db;
            _passwordHasher = hasher;
            _userIdentityProvider = userIdentityProvider;
            _jwtTokenService = jwtTokenService;
            _sessionService = sessionService;
            _authCookieService = authCookieService;
        }

        public async Task SignInAsync(SignInRequest request)
        {
            User? user = await _db.Users
           .FirstOrDefaultAsync(u => u.Login == request.Login);

            if (user is null)
            {
                throw new UnauthorizedUserException("User does not exist");
            }

            PasswordVerificationResult verifyPasswordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            bool isCorrectPassword = verifyPasswordResult != PasswordVerificationResult.Failed;

            if (!isCorrectPassword)
            {
                throw new UnauthorizedUserException("Incorrect password");
            }

            await StartSessionAsync(user);
        }

        public async Task SignUpAsync(SignUpRequest request)
        {
            bool userExists = await _db.Users
             .AnyAsync(user => user.Login == request.Login);

            if (userExists)
            {
                throw new UserAlreadyExistsException("User already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = request.Login,
                CreatedAt = DateTimeOffset.UtcNow
            };

            user.PasswordHash =
                _passwordHasher.HashPassword(user, request.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await StartSessionAsync(user);
        }

        public async Task LogOutAsync()
        {
            string? refreshToken = _authCookieService.GetRefreshToken();

            if (refreshToken is not null)
            {
                await _sessionService.DeleteSessionByRefreshTokenAsync(refreshToken);
            }

            _authCookieService.ClearAuthCookies();
        }

        public async Task RefreshTokenAsync()
        {
            string? refreshToken = _authCookieService.GetRefreshToken();

            if (refreshToken is null)
            {
                throw new UnauthorizedUserException("Failed to get refresh token");
            }

            Session session = await _sessionService.GetValidSessionByRefreshTokenAsync(refreshToken);

            string accessToken = _jwtTokenService.CreateAccessToken(session.User);
            _authCookieService.SetAccessToken(accessToken);
        }

        public async Task<CurrentUserInfoResponse> Me()
        {
            Guid userId = _userIdentityProvider.GetCurrentUserId();

            User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new UnauthorizedUserException("User not found");
            }

            return new CurrentUserInfoResponse(user.Login, user.CreatedAt);
        }

        private async Task StartSessionAsync(User user)
        {
            string accessToken = _jwtTokenService.CreateAccessToken(user);
            string refreshToken = await _sessionService.CreateSessionAsync(user);

            _authCookieService.SetAccessToken(accessToken);
            _authCookieService.SetRefreshToken(refreshToken);
        }
    }
}
