using System;
using MediatR;

namespace SFCoreProTM.Application.Features.States.Commands.DeleteState;

public sealed record DeleteStateCommand(Guid WorkspaceId, Guid ProjectId, Guid StateId, Guid ActorId) : IRequest;
