using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SFCoreProTM.Presentation.Options;

namespace SFCoreProTM.Presentation.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAt) CreateToken(Guid userId, string email, string displayName);
}

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly SigningCredentials _credentials;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public (string token, DateTime expiresAt) CreateToken(Guid userId, string email, string displayName)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_options.AccessTokenExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email ?? string.Empty),
            new("name", displayName ?? string.Empty),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: _credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, expires);
    }
}

