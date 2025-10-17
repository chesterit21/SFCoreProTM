using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.States;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.States.Commands.CreateState;

public sealed class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, StateDto>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IStateRepository _stateRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateStateCommandHandler(
        IWorkItemReadService workItemReadService,
        IStateRepository stateRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _stateRepository = stateRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _mapper = mapper;
    }

    public async Task<StateDto> Handle(CreateStateCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null || projectContext.WorkspaceId != request.WorkspaceId)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found in workspace '{request.WorkspaceId}'.");
        }

        if (!await _workItemReadService.IsProjectMemberAsync(request.ProjectId, request.ActorId, cancellationToken))
        {
            throw new ValidationException("Actor must be a project member.");
        }

        if (await _workItemReadService.StateNameExistsAsync(request.ProjectId, request.WorkspaceId, payload.Name, null, cancellationToken))
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
                null,
                cancellationToken);

            if (exists)
            {
                throw new ConflictException("State with the same external id and source already exists.");
            }
        }

        var color = ColorCode.FromHex(payload.ColorHex);
        var slug = Slug.Create(ToSlug(payload.Name));

        var state = State.Create(Guid.NewGuid(), request.WorkspaceId, request.ProjectId, payload.Name, color, slug, payload.Sequence);
        state.UpdateDetails(payload.Description, payload.Group, payload.IsTriage, payload.IsDefault, payload.Sequence);
        state.SetExternalReference(payload.ExternalSource, payload.ExternalId);

        var now = _dateTimeProvider.UtcNow;
        state.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _stateRepository.AddAsync(state, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (payload.IsDefault)
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
