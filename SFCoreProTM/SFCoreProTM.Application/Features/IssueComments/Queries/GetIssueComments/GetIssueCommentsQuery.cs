using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueComments;

public sealed record GetIssueCommentsQuery(Guid ProjectId, Guid IssueId) : IRequest<IReadOnlyCollection<IssueCommentDto>>;
