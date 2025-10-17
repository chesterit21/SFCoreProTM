using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.DTOs.Issues;

public sealed record IssueDto
{
    public Guid Id { get; init; }

    public Guid WorkspaceId { get; init; }

    public Guid ProjectId { get; init; }

    public string Name { get; init; } = string.Empty;

    public IssuePriority Priority { get; init; }

    public DateTime? StartDate { get; init; }

    public DateTime? TargetDate { get; init; }

    public Guid? StateId { get; init; }

    public Guid? ParentId { get; init; }

    public int SequenceId { get; init; }

    public double SortOrder { get; init; }

    public Guid? IssueTypeId { get; init; }

    public Guid? EstimatePointId { get; init; }

    public string? ExternalSource { get; init; }

    public string? ExternalId { get; init; }

    public IReadOnlyCollection<Guid> AssigneeIds { get; init; } = Array.Empty<Guid>();

    public IReadOnlyCollection<Guid> LabelIds { get; init; } = Array.Empty<Guid>();
}
