using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Invites.Commands.AcceptProjectInvite;

public sealed class AcceptProjectInviteCommandHandler : IRequestHandler<AcceptProjectInviteCommand, bool>
{
    private readonly IProjectInviteRepository _repository;
    private readonly IDateTimeProvider _clock;
    private readonly IUnitOfWork _uow;

    public AcceptProjectInviteCommandHandler(IProjectInviteRepository repository, IDateTimeProvider clock, IUnitOfWork uow)
    {
        _repository = repository;
        _clock = clock;
        _uow = uow;
    }

    public async Task<bool> Handle(AcceptProjectInviteCommand request, CancellationToken cancellationToken)
    {
        var invite = await _repository.GetByTokenAsync(request.Token, cancellationToken);
        if (invite is null || invite.Accepted)
        {
            return false;
        }

        var now = _clock.UtcNow;
        var member = ProjectMember.Create(Guid.NewGuid(), invite.WorkspaceId, invite.ProjectId, request.UserId, invite.Role, ProjectMemberPreferences.CreateDefault(), sortOrder: 0, isActive: true);
        member.SetAuditTrail(AuditTrail.Create(now, request.UserId, now, request.UserId, null));

        invite.Accept(now);
        invite.SetAuditTrail(AuditTrail.Create(invite.AuditTrail.CreatedAt, invite.AuditTrail.CreatedById, now, request.UserId, null));

        await using var tx = await _uow.BeginTransactionAsync(cancellationToken);
        await _repository.AddMemberAsync(member, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return true;
    }
}
