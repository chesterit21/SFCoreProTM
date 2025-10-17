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

namespace SFCoreProTM.Application.Features.Issues.Commands.UpdateIssue;

public sealed class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIssueSequenceService _issueSequenceService;
    private readonly IIssueHistoryService _issueHistoryService;
    private readonly IMapper _mapper;

    public UpdateIssueCommandHandler(
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

    public async Task<IssueDto> Handle(UpdateIssueCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found.");
        }

        var issue = await _issueRepository.GetByIdAsync(projectContext.WorkspaceId, request.ProjectId, request.IssueId, cancellationToken);
        if (issue is null)
        {
            throw new NotFoundException($"Issue '{request.IssueId}' was not found in project '{request.ProjectId}'.");
        }

        StateSummary? stateSummary = null;
        StateSummary? currentStateSummary = null;

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

        if (issue.StateId.HasValue)
        {
            currentStateSummary = await _workItemReadService.GetStateSummaryAsync(
                request.ProjectId,
                projectContext.WorkspaceId,
                issue.StateId.Value,
                cancellationToken);
        }

        if (stateSummary is null && !payload.StateId.HasValue)
        {
            stateSummary = currentStateSummary;
        }

        if (stateSummary is null)
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

        var targetStateId = stateSummary.Id;
        var stateChanged = !issue.StateId.HasValue || issue.StateId.Value != targetStateId;

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

        IReadOnlyCollection<Guid> assigneeIds = issue.AssigneeIds;
        if (payload.AssigneeIds is not null)
        {
            assigneeIds = await _workItemReadService
                .FilterAssignableMemberIdsAsync(request.ProjectId, payload.AssigneeIds, cancellationToken);
        }

        IReadOnlyCollection<Guid> labelIds = issue.LabelIds;
        if (payload.LabelIds is not null)
        {
            labelIds = await _workItemReadService
                .FilterLabelIdsAsync(request.ProjectId, projectContext.WorkspaceId, payload.LabelIds, cancellationToken);
        }

        var newIssueTypeId = payload.IssueTypeId ?? issue.IssueTypeId;
        if (payload.IssueTypeId is null && issue.IssueTypeId is null)
        {
            newIssueTypeId = await _workItemReadService.GetDefaultIssueTypeIdAsync(request.ProjectId, projectContext.WorkspaceId, cancellationToken);
        }

        var description = RichTextContent.Create(
            payload.DescriptionPlainText ?? issue.Description.PlainText,
            payload.DescriptionHtml ?? issue.Description.Html,
            payload.DescriptionBinary ?? issue.Description.Binary,
            payload.DescriptionJson ?? issue.Description.Json);

        var descriptionChanged = !Equals(description, issue.Description);

        var schedule = DateRange.Create(
            payload.StartDate ?? issue.Schedule.Start,
            payload.TargetDate ?? issue.Schedule.End);

        var now = _dateTimeProvider.UtcNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        var sortOrder = issue.SortOrder;
        if (stateChanged)
        {
            sortOrder = await _issueSequenceService.ReserveSortOrderAsync(request.ProjectId, targetStateId, cancellationToken);
        }

        issue.UpdateDetails(
            payload.Name ?? issue.Name,
            payload.Priority ?? issue.Priority,
            schedule,
            sortOrder,
            description,
            issue.IsDraft);

        issue.SetState(targetStateId);
        issue.SetHierarchy(payload.ParentId ?? issue.ParentId);
        issue.SetEstimate(payload.PointEstimate ?? issue.PointEstimate, payload.EstimatePointId ?? issue.EstimatePointId);
        issue.SetIssueType(newIssueTypeId);
        issue.SetExternalReference(payload.ExternalSource ?? issue.ExternalReference?.Source, payload.ExternalId ?? issue.ExternalReference?.Identifier);
        issue.ReplaceAssignees(assigneeIds);
        issue.ReplaceLabels(labelIds);

        if (stateChanged)
        {
            var isCompletedState = string.Equals(stateSummary.Group, "completed", StringComparison.OrdinalIgnoreCase);
            issue.MarkCompleted(isCompletedState ? now : null);
        }

        await _issueHistoryService.LogAsync(issue, request.ActorId, now, descriptionChanged, "issue:update", cancellationToken);

        var updatedAudit = AuditTrail.Create(
            issue.AuditTrail.CreatedAt,
            issue.AuditTrail.CreatedById,
            now,
            request.ActorId,
            issue.AuditTrail.DeletedAt);
        issue.SetAuditTrail(updatedAudit);

        await _issueRepository.UpdateAsync(issue, assigneeIds, labelIds, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<IssueDto>(issue);
    }

}
