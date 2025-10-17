using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces;

public sealed record ProjectContext(Guid ProjectId, Guid WorkspaceId, Guid? DefaultAssigneeId);

public sealed record StateSummary(Guid Id, string Group, bool IsTriage, bool IsDefault);

public sealed record IssueAssociations(IReadOnlyCollection<Guid> ModuleIds, Guid? CycleId);

public interface IWorkItemReadService
{
    Task<ProjectContext?> GetProjectContextAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<bool> StateExistsAsync(Guid projectId, Guid stateId, CancellationToken cancellationToken = default);

    Task<StateSummary?> GetStateSummaryAsync(Guid projectId, Guid workspaceId, Guid stateId, CancellationToken cancellationToken = default);

    Task<StateSummary?> GetInitialStateAsync(Guid projectId, Guid workspaceId, CancellationToken cancellationToken = default);

    Task<IssueAssociations> GetIssueAssociationsAsync(Guid projectId, Guid workspaceId, Guid issueId, CancellationToken cancellationToken = default);

    Task<bool> IssueExistsAsync(Guid projectId, Guid workspaceId, Guid issueId, CancellationToken cancellationToken = default);

    Task<bool> EstimatePointExistsAsync(Guid projectId, Guid workspaceId, Guid estimatePointId, CancellationToken cancellationToken = default);

    Task<bool> StateNameExistsAsync(Guid projectId, Guid workspaceId, string name, Guid? excludeStateId = null, CancellationToken cancellationToken = default);

    Task<bool> StateExternalIdExistsAsync(Guid projectId, Guid workspaceId, string externalSource, string externalId, Guid? excludeStateId = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Guid>> FilterAssignableMemberIdsAsync(
        Guid projectId,
        IEnumerable<Guid> candidateMemberIds,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Guid>> FilterLabelIdsAsync(
        Guid projectId,
        Guid workspaceId,
        IEnumerable<Guid> candidateLabelIds,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetDefaultIssueTypeIdAsync(Guid projectId, Guid workspaceId, CancellationToken cancellationToken = default);

    Task<bool> LabelNameExistsAsync(Guid projectId, Guid workspaceId, string name, Guid? excludeLabelId = null, CancellationToken cancellationToken = default);

    Task<bool> LabelExternalIdExistsAsync(Guid projectId, Guid workspaceId, string externalSource, string externalId, Guid? excludeLabelId = null, CancellationToken cancellationToken = default);

    Task<bool> IsProjectMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);

    Task<bool> IsProjectAdminAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
}
