using System;
using MediatR;
using SFCoreProTM.Application.DTOs.States;

namespace SFCoreProTM.Application.Features.States.Commands.CreateState;

public sealed record CreateStateCommand(Guid WorkspaceId, Guid ProjectId, Guid ActorId, CreateStateRequestDto Payload) : IRequest<StateDto>;
