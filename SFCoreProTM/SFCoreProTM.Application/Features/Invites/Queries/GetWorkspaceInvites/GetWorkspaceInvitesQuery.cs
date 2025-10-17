using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;

namespace SFCoreProTM.Application.Features.Invites.Queries.GetWorkspaceInvites;

public sealed record GetWorkspaceInvitesQuery(Guid WorkspaceId) : IRequest<IReadOnlyCollection<WorkspaceInviteAdminDto>>;

