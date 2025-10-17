using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueCommentById;

public sealed record GetIssueCommentByIdQuery(Guid ProjectId, Guid IssueId, Guid CommentId) : IRequest<IssueCommentDto>;
