using GraphForge.Api.Auth;
using GraphForge.Api.Database;
using GraphForge.Api.DTOs.Auth;
using GraphForge.Api.Models;
using GraphForge.Api.Services.UserIdentityProviderService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GraphForge.Api.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private const int AccessTokenExpirationTimeMinutes = 15;
        private const int RefreshTokenExpirationTimeMinutes = 30 * 24 * 60;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _db;
        private readonly AuthOptions _authOptions;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUserIdentityProvider _userIdentityProvider;


        public AuthService(
            AppDbContext db,
            AuthOptions authOptions,
            IHttpContextAccessor httpContextAccessor, 
            IPasswordHasher<User> hasher,
            IUserIdentityProvider userIdentityProvider)
        {
            _authOptions = authOptions;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = hasher;
            _userIdentityProvider = userIdentityProvider;
        }

        private async Task<string> CreateAccessTokenAsync(User user)
        {

            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Login),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "user"),
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_authOptions.Key)
            );

            JwtSecurityToken jwtToken = new JwtSecurityToken(
                    issuer: _authOptions.Issuer,
                    audience: _authOptions.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AccessTokenExpirationTimeMinutes)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return token;
        }

        public async Task ProvideAccessTokenAsync(User user)
        {
            string token = await CreateAccessTokenAsync(user);
            HttpResponse response = _httpContextAccessor.HttpContext!.Response;
            response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(AccessTokenExpirationTimeMinutes)
            });
        }

        private async Task CreateSessionAsync(Guid userId, string token)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(RefreshTokenExpirationTimeMinutes),
                RefreshTokenHash = HashRefreshToken(token)
            };

            _db.Sessions.Add(session);

            await _db.SaveChangesAsync();
        }

        private string CreateRefreshToken()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(64);
            string token = Convert.ToBase64String(bytes);
            return token;
        }

        private string HashRefreshToken(string token)
        {
            byte[] tokenHash = SHA256.HashData(
               Encoding.UTF8.GetBytes(token)
           );
            string hashedToken = Convert.ToHexString(tokenHash);
            return hashedToken;
        }

        public async Task ProvideSessionAsync(User user)
        {
            string refreshToken = CreateRefreshToken();
            HttpResponse response = _httpContextAccessor.HttpContext!.Response;
            response.Cookies.Append(
                "refresh_token",
                refreshToken,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(RefreshTokenExpirationTimeMinutes)
                }
            );
            await CreateSessionAsync(user.Id, refreshToken);
        }

        public async Task EndCurrentSessionAsync()
        {
            HttpRequest request = _httpContextAccessor.HttpContext!.Request;
            HttpResponse response = _httpContextAccessor.HttpContext!.Response;

            string? refreshToken = request.Cookies["refresh_token"];

            if (refreshToken is not null)
            {
                string hash = HashRefreshToken(refreshToken);

                Session? session = await _db.Sessions.FirstOrDefaultAsync(s => s.RefreshTokenHash == hash);

                if (session is not null)
                {
                    _db.Sessions.Remove(session);
                    await _db.SaveChangesAsync();
                }
            }

            response.Cookies.Delete("access_token");
            response.Cookies.Delete("refresh_token");
        }

        public async Task RefreshAccessTokenAsync()
        {
            HttpRequest request = _httpContextAccessor.HttpContext!.Request;

            string? refreshToken = request.Cookies["refresh_token"];

            if (refreshToken is null)
            {
                throw new UnauthorizedUserException("Failed to get refresh token");
            }

            string hash = HashRefreshToken(refreshToken);

            Session? session = await _db.Sessions
                .FirstOrDefaultAsync(s => s.RefreshTokenHash == hash);

            if (session is null)
            {
                throw new UnauthorizedUserException("Session not found");
            }

            if (session.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                _db.Sessions.Remove(session);
                await _db.SaveChangesAsync();

                throw new UnauthorizedUserException("Session expired");
            }

            User? user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == session.UserId);

            if (user is null)
            {
                _db.Sessions.Remove(session);
                await _db.SaveChangesAsync();

                throw new UnauthorizedUserException("User not found");
            }

            await ProvideAccessTokenAsync(user);
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

            await ProvideAccessTokenAsync(user);
            await ProvideSessionAsync(user);
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

            await ProvideAccessTokenAsync(user);
            await ProvideSessionAsync(user);
        }

        public async Task LogOutAsync()
        {
            await EndCurrentSessionAsync();
        }

        public async Task RefreshTokenAsync()
        {
            await RefreshAccessTokenAsync();
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
    }
}
