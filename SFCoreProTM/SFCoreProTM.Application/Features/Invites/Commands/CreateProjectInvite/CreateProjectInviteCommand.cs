using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;

namespace SFCoreProTM.Application.Features.Invites.Commands.CreateProjectInvite;

public sealed record CreateProjectInviteCommand(Guid WorkspaceId, Guid ProjectId, Guid ActorId, string Email, int Role, string? Message) : IRequest<ProjectInviteCreatedDto>;

