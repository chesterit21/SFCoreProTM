using System;

namespace SFCoreProTM.Application.DTOs.Invites;

public sealed class WorkspaceInviteAdminDto
{
    public Guid WorkspaceId { get; init; }
    public string Email { get; init; } = string.Empty;
    public int Role { get; init; }
    public bool Accepted { get; init; }
    public Guid Token { get; init; }
}

