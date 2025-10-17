using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;

namespace SFCoreProTM.Application.Features.Invites.Commands.CreateWorkspaceInvite;

public sealed record CreateWorkspaceInviteCommand(Guid WorkspaceId, Guid ActorId, string Email, int Role, string? Message) : IRequest<WorkspaceInviteCreatedDto>;

