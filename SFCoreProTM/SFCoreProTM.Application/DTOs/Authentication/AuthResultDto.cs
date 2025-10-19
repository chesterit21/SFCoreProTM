using System;

namespace SFCoreProTM.Application.DTOs.Authentication;

public sealed class AuthResultDto
{
    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public DateTime? LastLoginAt { get; init; }

    public bool IsPasswordAutoset { get; init; }
    public Guid? LastWorkspaceId { get; init; }
}
