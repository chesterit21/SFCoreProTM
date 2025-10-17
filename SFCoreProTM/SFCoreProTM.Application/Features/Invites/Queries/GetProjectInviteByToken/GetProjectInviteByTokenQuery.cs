using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Invites;

namespace SFCoreProTM.Application.Features.Invites.Queries.GetProjectInviteByToken;

public sealed record GetProjectInviteByTokenQuery(Guid Token) : IRequest<ProjectInviteCreatedDto?>;

