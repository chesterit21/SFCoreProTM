using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.Issues.Commands.CreateIssue;

public sealed class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIssueSequenceService _issueSequenceService;
    private readonly IIssueHistoryService _issueHistoryService;
    private readonly IMapper _mapper;

    public CreateIssueCommandHandler(
        IIssueRepository issueRepository,
        IWorkItemReadService workItemReadService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IIssueSequenceService issueSequenceService,
        IIssueHistoryService issueHistoryService,
        IMapper mapper)
    {
        _issueRepository = issueRepository;
        _workItemReadService = workItemReadService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _issueSequenceService = issueSequenceService;
        _issueHistoryService = issueHistoryService;
        _mapper = mapper;
    }

    public async Task<IssueDto> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found.");
        }

        if (!string.IsNullOrWhiteSpace(payload.ExternalId) && !string.IsNullOrWhiteSpace(payload.ExternalSource))
        {
            var duplicateId = await _issueRepository.GetIdByExternalIdAsync(
                projectContext.WorkspaceId,
                request.ProjectId,
                payload.ExternalSource!,
                payload.ExternalId!,
                cancellationToken);

            if (duplicateId.HasValue)
            {
                throw new ConflictException($"Issue with external reference already exists. Id: {duplicateId}");
            }
        }

        StateSummary? stateSummary = null;

        if (payload.StateId.HasValue)
        {
            stateSummary = await _workItemReadService.GetStateSummaryAsync(
                request.ProjectId,
                projectContext.WorkspaceId,
                payload.StateId.Value,
                cancellationToken);

            if (stateSummary is null)
            {
                throw new NotFoundException($"State '{payload.StateId}' does not exist in project '{request.ProjectId}'.");
            }
        }
        else
        {
            stateSummary = await _workItemReadService.GetInitialStateAsync(
                request.ProjectId,
                projectContext.WorkspaceId,
                cancellationToken);

            if (stateSummary is null)
            {
                throw new ValidationException("Project does not have any available states.");
            }
        }

        var targetStateId = stateSummary?.Id;

        if (payload.ParentId.HasValue)
        {
            var parentExists = await _workItemReadService.IssueExistsAsync(
                request.ProjectId,
                projectContext.WorkspaceId,
                payload.ParentId.Value,
                cancellationToken);

            if (!parentExists)
            {
                throw new NotFoundException($"Parent issue '{payload.ParentId}' was not found in project '{request.ProjectId}'.");
            }
        }

        if (payload.EstimatePointId.HasValue)
        {
            var estimatePointExists = await _workItemReadService.EstimatePointExistsAsync(
                request.ProjectId,
                projectContext.WorkspaceId,
                payload.EstimatePointId.Value,
                cancellationToken);

            if (!estimatePointExists)
            {
                throw new NotFoundException($"Estimate point '{payload.EstimatePointId}' was not found in project '{request.ProjectId}'.");
            }
        }

        var assigneeIds = await _workItemReadService
            .FilterAssignableMemberIdsAsync(request.ProjectId, payload.AssigneeIds, cancellationToken);

        if (assigneeIds.Count == 0 && projectContext.DefaultAssigneeId.HasValue)
        {
            assigneeIds = await _workItemReadService
                .FilterAssignableMemberIdsAsync(request.ProjectId, new[] { projectContext.DefaultAssigneeId.Value }, cancellationToken);
        }

        var labelIds = await _workItemReadService
            .FilterLabelIdsAsync(request.ProjectId, projectContext.WorkspaceId, payload.LabelIds, cancellationToken);

        var issueTypeId = payload.IssueTypeId;
        if (!issueTypeId.HasValue)
        {
            issueTypeId = await _workItemReadService.GetDefaultIssueTypeIdAsync(request.ProjectId, projectContext.WorkspaceId, cancellationToken);
        }

        var description = RichTextContent.Create(
            payload.DescriptionPlainText,
            payload.DescriptionHtml,
            payload.DescriptionBinary,
            payload.DescriptionJson);

        var schedule = DateRange.Create(payload.StartDate, payload.TargetDate);
        var now = _dateTimeProvider.UtcNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var reservation = await _issueSequenceService.ReserveAsync(request.ProjectId, targetStateId, cancellationToken);

        var issue = Issue.Create(
            Guid.NewGuid(),
            projectContext.WorkspaceId,
            request.ProjectId,
            payload.Name,
            payload.Priority,
            reservation.SequenceId,
            schedule,
            reservation.SortOrder,
            description);

        issue.SetAuditTrail(AuditTrail.Create(now, request.ActorId, now, request.ActorId, null));
        issue.SetEstimate(payload.PointEstimate, payload.EstimatePointId);
        issue.SetState(targetStateId);

        var isCompletedState = stateSummary != null && string.Equals(stateSummary.Group, "completed", StringComparison.OrdinalIgnoreCase);
        issue.MarkCompleted(isCompletedState ? now : null);

        issue.SetHierarchy(payload.ParentId);
        issue.SetExternalReference(payload.ExternalSource, payload.ExternalId);
        issue.SetIssueType(issueTypeId);

        foreach (var assigneeId in assigneeIds)
        {
            issue.Assign(assigneeId);
        }

        foreach (var labelId in labelIds)
        {
            issue.TagLabel(labelId);
        }

        await _issueHistoryService.LogAsync(issue, request.ActorId, now, includeDescription: true, changeSource: "issue:create", cancellationToken: cancellationToken);

        await _issueRepository.AddAsync(issue, assigneeIds, labelIds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<IssueDto>(issue);
    }

}
