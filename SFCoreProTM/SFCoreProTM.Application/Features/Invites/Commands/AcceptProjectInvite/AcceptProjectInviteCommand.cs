using System;
using MediatR;

namespace SFCoreProTM.Application.Features.Invites.Commands.AcceptProjectInvite;

public sealed record AcceptProjectInviteCommand(Guid Token, Guid UserId) : IRequest<bool>;

