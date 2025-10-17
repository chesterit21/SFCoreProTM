using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Services;

public sealed class IssueHistoryService : IIssueHistoryService
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IIssueVersionRepository _issueVersionRepository;
    private readonly IIssueDescriptionVersionRepository _issueDescriptionVersionRepository;

    public IssueHistoryService(
        IWorkItemReadService workItemReadService,
        IIssueVersionRepository issueVersionRepository,
        IIssueDescriptionVersionRepository issueDescriptionVersionRepository)
    {
        _workItemReadService = workItemReadService;
        _issueVersionRepository = issueVersionRepository;
        _issueDescriptionVersionRepository = issueDescriptionVersionRepository;
    }

    public async Task LogAsync(Issue issue, Guid actorId, DateTime recordedAt, bool includeDescription, string? changeSource = null, CancellationToken cancellationToken = default)
    {
        var associations = await _workItemReadService.GetIssueAssociationsAsync(issue.ProjectId, issue.WorkspaceId, issue.Id, cancellationToken);

        var version = IssueVersion.Create(
            Guid.NewGuid(),
            issue.WorkspaceId,
            issue.ProjectId,
            issue.Id,
            issue.Name,
            issue.Priority,
            issue.SequenceId,
            recordedAt,
            actorId);

        version.SetContext(
            issue.ParentId,
            issue.StateId,
            issue.EstimatePointId,
            issue.Schedule.Start,
            issue.Schedule.End,
            issue.SortOrder,
            issue.IsDraft);

        version.SetLifecycle(issue.CompletedAt, issue.ArchivedAt);
        version.SetExternalReference(issue.ExternalReference?.Source, issue.ExternalReference?.Identifier);
        version.SetIssueType(issue.IssueTypeId);
        version.SetCycle(associations.CycleId);
        version.AddAssignees(issue.AssigneeIds);
        version.AddLabels(issue.LabelIds);
        version.AddModules(associations.ModuleIds);
        version.SetProperties(StructuredData.FromJson(null), CreateMetadata(changeSource));

        await _issueVersionRepository.AddAsync(version, cancellationToken);

        if (includeDescription)
        {
            var descriptionVersion = IssueDescriptionVersion.Create(
                Guid.NewGuid(),
                issue.WorkspaceId,
                issue.ProjectId,
                issue.Id,
                actorId,
                recordedAt,
                issue.Description);

            await _issueDescriptionVersionRepository.AddAsync(descriptionVersion, cancellationToken);
        }
    }

    private static StructuredData CreateMetadata(string? changeSource)
    {
        if (string.IsNullOrWhiteSpace(changeSource))
        {
            return StructuredData.FromJson(null);
        }

        var payload = JsonSerializer.Serialize(new { source = changeSource });
        return StructuredData.FromJson(payload);
    }
}
