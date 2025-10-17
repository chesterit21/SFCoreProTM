using System;
using System.Collections.Generic;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.DTOs.Issues;

public sealed class CreateIssueCommentRequestDto
{
    public Guid ActorId { get; set; }

    public string? CommentPlainText { get; set; }

    public string? CommentHtml { get; set; }

    public string? CommentJson { get; set; }

    public byte[]? CommentBinary { get; set; }

    public IssueCommentAccess Access { get; set; } = IssueCommentAccess.Internal;

    public IList<string> Attachments { get; set; } = new List<string>();

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }
}
