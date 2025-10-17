using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IIssueRepository
{
    Task<bool> ExistsByExternalIdAsync(
        Guid workspaceId,
        Guid projectId,
        string externalSource,
        string externalId,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetIdByExternalIdAsync(
        Guid workspaceId,
        Guid projectId,
        string externalSource,
        string externalId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(Guid workspaceId, Guid projectId, Guid issueId, CancellationToken cancellationToken = default);

    Task AddAsync(Issue issue, IEnumerable<Guid> assigneeIds, IEnumerable<Guid> labelIds, CancellationToken cancellationToken = default);

    Task<int> GetNextSequenceIdAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<double> GetNextSortOrderAsync(Guid projectId, Guid? stateId, CancellationToken cancellationToken = default);

    Task<Issue?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid issueId, CancellationToken cancellationToken = default);

    Task UpdateAsync(Issue issue, IEnumerable<Guid> assigneeIds, IEnumerable<Guid> labelIds, CancellationToken cancellationToken = default);

    Task DeleteAsync(Issue issue, CancellationToken cancellationToken = default);

    Task<bool> HasIssuesInStateAsync(Guid workspaceId, Guid projectId, Guid stateId, CancellationToken cancellationToken = default);
}
