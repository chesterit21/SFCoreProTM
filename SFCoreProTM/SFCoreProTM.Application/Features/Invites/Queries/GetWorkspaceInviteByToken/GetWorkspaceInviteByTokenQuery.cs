using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;

namespace SFCoreProTM.Application.Features.Invites.Queries.GetWorkspaceInviteByToken;

public sealed record GetWorkspaceInviteByTokenQuery(Guid Token) : IRequest<WorkspaceInviteInfoDto?>;

