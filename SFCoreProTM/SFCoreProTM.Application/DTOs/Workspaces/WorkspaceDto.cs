using System;

namespace SFCoreProTM.Application.DTOs.Workspaces;

public sealed class WorkspaceDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Logo { get; init; }
    public Guid? LogoAssetId { get; init; }
    public Guid OwnerId { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string? OrganizationSize { get; init; }
    public string Timezone { get; init; } = "UTC";
    public string? BackgroundColor { get; init; }
}