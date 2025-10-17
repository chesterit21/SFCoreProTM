using System;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.DTOs.States;

public sealed record StateDto
{
    public Guid Id { get; init; }

    public Guid WorkspaceId { get; init; }

    public Guid ProjectId { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string ColorHex { get; init; } = "#000000";

    public string Slug { get; init; } = string.Empty;

    public string Group { get; init; } = "backlog";

    public bool IsTriage { get; init; }

    public bool IsDefault { get; init; }

    public int Sequence { get; init; }

    public string? ExternalSource { get; init; }

    public string? ExternalId { get; init; }
}
