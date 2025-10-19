using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Workspaces;

namespace SFCoreProTM.Application.Features.Workspaces.Queries.GetById;

public sealed record GetWorkspaceByIdQuery(Guid WorkspaceId) : IRequest<WorkspaceDto?>;