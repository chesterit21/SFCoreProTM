using System;

namespace SFCoreProTM.Presentation.Models.Authentication;

public sealed class AuthResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public DateTime? LastLoginAt { get; init; }
    public bool IsPasswordAutoset { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}

