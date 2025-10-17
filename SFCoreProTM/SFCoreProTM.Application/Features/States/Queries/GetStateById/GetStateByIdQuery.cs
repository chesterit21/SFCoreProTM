using System;
using MediatR;
using SFCoreProTM.Application.DTOs.States;

namespace SFCoreProTM.Application.Features.States.Queries.GetStateById;

public sealed record GetStateByIdQuery(Guid WorkspaceId, Guid ProjectId, Guid StateId) : IRequest<StateDto>;
