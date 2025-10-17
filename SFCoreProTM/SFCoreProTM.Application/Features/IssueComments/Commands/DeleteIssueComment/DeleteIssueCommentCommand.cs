using System;
using MediatR;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.DeleteIssueComment;

public sealed record DeleteIssueCommentCommand(Guid ProjectId, Guid IssueId, Guid CommentId, Guid ActorId) : IRequest<Unit>;
