using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.Mapping.Requests.Issues;

public sealed class UpdateIssueCommentRequest
{
    [Required]
    public Guid ActorId { get; set; }

    public string? CommentPlainText { get; set; }

    public string? CommentHtml { get; set; }

    public string? CommentJson { get; set; }

    public byte[]? CommentBinary { get; set; }

    public IssueCommentAccess? Access { get; set; }

    public IList<string>? Attachments { get; set; }

    public string? ExternalSource { get; set; }

    public string? ExternalId { get; set; }
}

