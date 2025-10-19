using System;

namespace SFCoreProTM.Application.DTOs.Users;

public sealed class CurrentUserDto
{
    public Guid? UserId { get; init; }
    public string? Email { get; init; } = string.Empty;
    public string? DisplayName { get; init; } = string.Empty;
    public DateTime? LastLoginAt { get; init; }
    public bool? IsPasswordAutoset { get; init; } = false;
}

