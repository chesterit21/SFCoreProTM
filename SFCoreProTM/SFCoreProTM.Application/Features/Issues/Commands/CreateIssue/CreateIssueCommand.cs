using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Issues;

namespace SFCoreProTM.Application.Features.Issues.Commands.CreateIssue;

public sealed record CreateIssueCommand(Guid ProjectId, Guid ActorId, CreateIssueRequestDto Payload) : IRequest<IssueDto>;
