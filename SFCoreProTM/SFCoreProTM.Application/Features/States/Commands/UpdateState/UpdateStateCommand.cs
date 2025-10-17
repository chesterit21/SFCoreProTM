using System;
using MediatR;
using SFCoreProTM.Application.DTOs.States;

namespace SFCoreProTM.Application.Features.States.Commands.UpdateState;

public sealed record UpdateStateCommand(Guid WorkspaceId, Guid ProjectId, Guid StateId, Guid ActorId, UpdateStateRequestDto Payload) : IRequest<StateDto>;
