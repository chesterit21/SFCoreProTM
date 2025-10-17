using System;
using MediatR;

namespace SFCoreProTM.Application.Features.Invites.Commands.AcceptWorkspaceInvite;

public sealed record AcceptWorkspaceInviteCommand(Guid Token, Guid UserId) : IRequest<bool>;

