using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.States;

namespace SFCoreProTM.Application.Features.States.Queries.GetStates;

public sealed record GetStatesQuery(Guid WorkspaceId, Guid ProjectId) : IRequest<IReadOnlyCollection<StateDto>>;
