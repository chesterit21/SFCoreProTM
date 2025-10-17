using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.DTOs.Issues;

public sealed record IssueCommentDto
{
    public Guid Id { get; init; }

    public Guid IssueId { get; init; }

    public Guid WorkspaceId { get; init; }

    public Guid ProjectId { get; init; }

    public Guid? ActorId { get; init; }

    public IssueCommentAccess Access { get; init; }

    public string? CommentPlainText { get; init; }

    public string? CommentHtml { get; init; }

    public string? CommentJson { get; init; }

    public IReadOnlyCollection<string> Attachments { get; init; } = Array.Empty<string>();

    public string? ExternalSource { get; init; }

    public string? ExternalId { get; init; }

    public DateTime? EditedAt { get; init; }
}
