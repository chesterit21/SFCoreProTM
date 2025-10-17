using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.Issues.Commands.UpdateIssue;

public sealed record UpdateIssueCommand(Guid ProjectId, Guid IssueId, Guid ActorId, UpdateIssueRequestDto Payload) : IRequest<IssueDto>;
