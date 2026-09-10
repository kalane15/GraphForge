using GraphForge.Api.Database;
using GraphForge.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace GraphForge.Api.Services.AuthService;

public class SessionService : ISessionService
{
    private const int RefreshTokenExpirationTimeMinutes = 30 * 24 * 60;

    private readonly AppDbContext _db;

    public SessionService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<string> CreateSessionAsync(User user)
    {
        string refreshToken = CreateRefreshToken();

        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(RefreshTokenExpirationTimeMinutes),
            RefreshTokenHash = HashRefreshToken(refreshToken)
        };

        _db.Sessions.Add(session);
        await _db.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<Session> GetValidSessionByRefreshTokenAsync(string refreshToken)
    {
        string hash = HashRefreshToken(refreshToken);

        Session? session = await _db.Sessions
            .Include(s => s.User)
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

        return session;
    }

    public async Task DeleteSessionByRefreshTokenAsync(string refreshToken)
    {
        string hash = HashRefreshToken(refreshToken);

        Session? session = await _db.Sessions
            .FirstOrDefaultAsync(s => s.RefreshTokenHash == hash);

        if (session is null)
        {
            return;
        }

        _db.Sessions.Remove(session);
        await _db.SaveChangesAsync();
    }

    private static string CreateRefreshToken()
    {
        byte[] bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private static string HashRefreshToken(string token)
    {
        byte[] tokenHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(token)
        );

        return Convert.ToHexString(tokenHash);
    }
}
