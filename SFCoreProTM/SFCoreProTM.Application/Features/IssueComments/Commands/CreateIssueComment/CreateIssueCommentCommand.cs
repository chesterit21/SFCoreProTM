using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.CreateIssueComment;

public sealed record CreateIssueCommentCommand(Guid ProjectId, Guid IssueId, CreateIssueCommentRequestDto Payload) : IRequest<IssueCommentDto>;
