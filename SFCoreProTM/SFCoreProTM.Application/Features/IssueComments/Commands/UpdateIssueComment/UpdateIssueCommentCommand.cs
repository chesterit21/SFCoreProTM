using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.UpdateIssueComment;

public sealed record UpdateIssueCommentCommand(Guid ProjectId, Guid IssueId, Guid CommentId, UpdateIssueCommentRequestDto Payload) : IRequest<IssueCommentDto>;
