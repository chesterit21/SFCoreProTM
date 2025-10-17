using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.States;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.States.Commands.UpdateState;

public sealed class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, StateDto>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IStateRepository _stateRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public UpdateStateCommandHandler(
        IWorkItemReadService workItemReadService,
        IStateRepository stateRepository,
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _stateRepository = stateRepository;
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<StateDto> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload ?? throw new ValidationException("Payload is required");

        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null || projectContext.WorkspaceId != request.WorkspaceId)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found in workspace '{request.WorkspaceId}'.");
        }

        var state = await _stateRepository.GetByIdAsync(request.WorkspaceId, request.ProjectId, request.StateId, cancellationToken);
        if (state is null)
        {
            throw new NotFoundException($"State '{request.StateId}' was not found.");
        }

        if (payload.Name is not null &&
            await _workItemReadService.StateNameExistsAsync(request.ProjectId, request.WorkspaceId, payload.Name, request.StateId, cancellationToken))
        {
            throw new ConflictException($"State name '{payload.Name}' already exists in this project.");
        }

        if (!string.IsNullOrWhiteSpace(payload.ExternalSource) && !string.IsNullOrWhiteSpace(payload.ExternalId))
        {
            var exists = await _workItemReadService.StateExternalIdExistsAsync(
                request.ProjectId,
                request.WorkspaceId,
                payload.ExternalSource!,
                payload.ExternalId!,
                request.StateId,
                cancellationToken);

            if (exists)
            {
                throw new ConflictException("State with the same external id and source already exists.");
            }
        }

        if (payload.Name is not null)
        {
            state.Rename(payload.Name, Slug.Create(ToSlug(payload.Name)));
        }

        if (payload.ColorHex is not null)
        {
            state.SetColor(ColorCode.FromHex(payload.ColorHex));
        }

        var description = payload.Description ?? state.Description;
        var group = payload.Group ?? state.Group;
        var isTriage = payload.IsTriage ?? state.IsTriage;
        var isDefault = payload.IsDefault ?? state.IsDefault;
        var sequence = payload.Sequence ?? state.Sequence;

        state.UpdateDetails(description, group, isTriage, isDefault, sequence);
        state.SetExternalReference(payload.ExternalSource ?? state.ExternalReference?.Source, payload.ExternalId ?? state.ExternalReference?.Identifier);

        var now = _dateTimeProvider.UtcNow;
        var audit = AuditTrail.Create(state.AuditTrail.CreatedAt, state.AuditTrail.CreatedById, now, request.ActorId, state.AuditTrail.DeletedAt);
        state.SetAuditTrail(audit);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _stateRepository.UpdateAsync(state, cancellationToken);

        if (payload.IsDefault.HasValue && payload.IsDefault.Value)
        {
            await _stateRepository.SetDefaultAsync(request.WorkspaceId, request.ProjectId, state.Id, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<StateDto>(state);
    }

    private static string ToSlug(string input)
    {
        var lower = input.Trim().ToLowerInvariant();
        return string.Join('-', lower.Split(new[] { ' ', '_' }, StringSplitOptions.RemoveEmptyEntries));
    }
}
