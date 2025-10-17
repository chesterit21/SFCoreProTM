using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;

namespace SFCoreProTM.Application.Features.Invites.Queries.GetProjectInvites;

public sealed record GetProjectInvitesQuery(Guid WorkspaceId, Guid ProjectId) : IRequest<IReadOnlyCollection<ProjectInviteAdminDto>>;

