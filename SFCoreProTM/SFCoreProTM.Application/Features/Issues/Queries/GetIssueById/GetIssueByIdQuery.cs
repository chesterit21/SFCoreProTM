using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.Issues.Queries.GetIssueById;

public sealed record GetIssueByIdQuery(Guid ProjectId, Guid IssueId) : IRequest<IssueDto>;
