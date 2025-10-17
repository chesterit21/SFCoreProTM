using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FluentValidation;
using SFCoreProTM.Application.DTOs.Invites;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Invites.Commands.CreateProjectInvite;

public sealed class CreateProjectInviteCommandHandler : IRequestHandler<CreateProjectInviteCommand, ProjectInviteCreatedDto>
{
    private readonly IProjectInviteRepository _repository;
    private readonly IUnitOfWork _uow;
    private readonly IDateTimeProvider _clock;
    private readonly SFCoreProTM.Application.Interfaces.IWorkItemReadService _workItemReadService;

    public CreateProjectInviteCommandHandler(IProjectInviteRepository repository, IUnitOfWork uow, IDateTimeProvider clock, SFCoreProTM.Application.Interfaces.IWorkItemReadService workItemReadService)
    {
        _repository = repository;
        _uow = uow;
        _clock = clock;
        _workItemReadService = workItemReadService;
    }

    public async Task<ProjectInviteCreatedDto> Handle(CreateProjectInviteCommand request, CancellationToken cancellationToken)
    {
        if (!await _workItemReadService.IsProjectAdminAsync(request.ProjectId, request.ActorId, cancellationToken))
        {
            throw new ValidationException("Actor must be a project admin to invite members.");
        }

        var email = EmailAddress.Create(request.Email.Trim().ToLowerInvariant());
        var token = Guid.NewGuid();
        var role = (ProjectRole)request.Role;
        var invite = ProjectMemberInvite.Create(Guid.NewGuid(), request.WorkspaceId, request.ProjectId, email, token, role);
        if (!string.IsNullOrWhiteSpace(request.Message))
        {
            invite.UpdateMessage(request.Message);
        }

        var now = _clock.UtcNow;
        invite.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));

        await using var tx = await _uow.BeginTransactionAsync(cancellationToken);
        await _repository.AddAsync(invite, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return new ProjectInviteCreatedDto
        {
            WorkspaceId = request.WorkspaceId,
            ProjectId = request.ProjectId,
            Email = email.Value,
            Role = (int)role,
            Token = token,
        };
    }
}
