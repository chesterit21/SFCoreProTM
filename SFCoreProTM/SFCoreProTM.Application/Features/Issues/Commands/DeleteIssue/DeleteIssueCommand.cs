using System;
using MediatR;

namespace SFCoreProTM.Application.Features.Issues.Commands.DeleteIssue;

public sealed record DeleteIssueCommand(Guid ProjectId, Guid IssueId, Guid ActorId) : IRequest;
