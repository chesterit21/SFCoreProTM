using System;

namespace SFCoreProTM.Application.DTOs.Labels;

public sealed class LabelDto
{
    public Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Color { get; init; } = "#000000";
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ExternalSource { get; init; }
    public string? ExternalIdentifier { get; init; }
}
