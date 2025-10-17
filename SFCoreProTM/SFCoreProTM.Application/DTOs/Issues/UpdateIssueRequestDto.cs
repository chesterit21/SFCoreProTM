using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.DTOs.Issues;

public sealed class UpdateIssueRequestDto
{
    public string? Name { get; set; }

    public string? DescriptionPlainText { get; set; }

    public string? DescriptionHtml { get; set; }

    public string? DescriptionJson { get; set; }

    public byte[]? DescriptionBinary { get; set; }

    public IssuePriority? Priority { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? TargetDate { get; set; }

    public Guid? ParentId { get; set; }

    public Guid? StateId { get; set; }

    public Guid? EstimatePointId { get; set; }

    public int? PointEstimate { get; set; }

    public Guid? IssueTypeId { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }

    public IList<Guid>? AssigneeIds { get; set; }

    public IList<Guid>? LabelIds { get; set; }
}
