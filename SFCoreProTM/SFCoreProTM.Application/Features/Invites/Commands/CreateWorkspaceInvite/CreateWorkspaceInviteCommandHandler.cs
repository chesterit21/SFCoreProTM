using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using SFCoreProTM.Application.DTOs.Invites;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Invites.Commands.CreateWorkspaceInvite;

public sealed class CreateWorkspaceInviteCommandHandler : IRequestHandler<CreateWorkspaceInviteCommand, WorkspaceInviteCreatedDto>
{
    private readonly IWorkspaceInviteRepository _repository;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;
    private readonly SFCoreProTM.Application.Interfaces.IWorkspaceReadService _workspaceReadService;

    public CreateWorkspaceInviteCommandHandler(IWorkspaceInviteRepository repository, IUnitOfWork uow, IDateTimeProvider clock, SFCoreProTM.Application.Interfaces.IWorkspaceReadService workspaceReadService)
    {
        _repository = repository;
        _uow = uow;
        _clock = clock;
        _workspaceReadService = workspaceReadService;
    }

    public async Task<WorkspaceInviteCreatedDto> Handle(CreateWorkspaceInviteCommand request, CancellationToken cancellationToken)
    {

        if (!await _workspaceReadService.IsAdminAsync(request.WorkspaceId, request.ActorId, cancellationToken))
        {
            throw new ValidationException("Actor must be a workspace admin to invite members.");
        }

        var email = EmailAddress.Create(request.Email.Trim().ToLowerInvariant());
        var token = Guid.NewGuid();
        var role = (WorkspaceRole)request.Role;
        var invite = WorkspaceMemberInvite.Create(Guid.NewGuid(), request.WorkspaceId, email, token, role);
        if (!string.IsNullOrWhiteSpace(request.Message))
        {
            invite.AddMessage(request.Message);
        }

        var now = _clock.UtcNow;
        invite.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));

        await using var tx = await _uow.BeginTransactionAsync(cancellationToken);
        await _repository.AddAsync(invite, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return new WorkspaceInviteCreatedDto
        {
            WorkspaceId = request.WorkspaceId,
            Email = email.Value,
            Role = (int)role,
            Token = token,
        };
    }
}
