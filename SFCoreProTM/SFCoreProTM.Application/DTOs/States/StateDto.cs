using System;

namespace SFCoreProTM.Application.DTOs.States;

public sealed class StateDto
{
    public Guid Id { get; init; }
    public Guid WorkspaceId { get; init; }
    public Guid ProjectId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Color { get; init; } = "#000000";
    public string Slug { get; init; } = string.Empty;
    public int Sequence { get; init; }
    public string Group { get; init; } = "backlog";
    public bool IsTriage { get; init; }
    public bool IsDefault { get; init; }
    public string? ExternalSource { get; init; }
    public string? ExternalIdentifier { get; init; }
}
