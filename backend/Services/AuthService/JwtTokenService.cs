using GraphForge.Api.Auth;
using GraphForge.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GraphForge.Api.Services.AuthService;

public class JwtTokenService : IJwtTokenService
{
    private const int AccessTokenExpirationTimeMinutes = 15;

    private readonly AuthOptions _authOptions;

    public JwtTokenService(AuthOptions authOptions)
    {
        _authOptions = authOptions;
    }

    public string CreateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
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
            expires: DateTime.UtcNow.AddMinutes(AccessTokenExpirationTimeMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}
