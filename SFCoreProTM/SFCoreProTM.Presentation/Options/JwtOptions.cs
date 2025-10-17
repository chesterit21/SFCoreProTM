using System;

namespace SFCoreProTM.Presentation.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; } = 60;
    public string CookieName { get; set; } = "auth_token";
    public bool CookieSecure { get; set; } = true;
}

