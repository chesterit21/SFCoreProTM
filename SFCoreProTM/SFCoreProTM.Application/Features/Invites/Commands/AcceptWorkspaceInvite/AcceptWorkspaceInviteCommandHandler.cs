using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Invites.Commands.AcceptWorkspaceInvite;

public sealed class AcceptWorkspaceInviteCommandHandler : IRequestHandler<AcceptWorkspaceInviteCommand, bool>
{
    private readonly IWorkspaceInviteRepository _repository;
    private readonly IDateTimeProvider _clock;
    private readonly IUnitOfWork _uow;

    public AcceptWorkspaceInviteCommandHandler(IWorkspaceInviteRepository repository, IDateTimeProvider clock, IUnitOfWork uow)
    {
        _repository = repository;
        _clock = clock;
        _uow = uow;
    }

    public async Task<bool> Handle(AcceptWorkspaceInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await _repository.GetByTokenAsync(request.Token, cancellationToken);
        if (invite is null || invite.Accepted)
        {
            return false;
        }

        var now = _clock.UtcNow;

        // Create workspace member from invite
        var preferences = WorkspaceMemberPreferences.CreateDefault();
        var member = WorkspaceMember.Create(Guid.NewGuid(), invite.WorkspaceId, request.UserId, invite.Role, preferences, isActive: true);
        member.SetAuditTrail(AuditTrail.Create(now, request.UserId, now, request.UserId, null));

        // Mark invite accepted
        invite.Accept(now);
        invite.SetAuditTrail(AuditTrail.Create(invite.AuditTrail.CreatedAt, invite.AuditTrail.CreatedById, now, request.UserId, null));

        await using var tx = await _uow.BeginTransactionAsync(cancellationToken);
        await _repository.AddMemberAsync(member, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return true;
    }
}

