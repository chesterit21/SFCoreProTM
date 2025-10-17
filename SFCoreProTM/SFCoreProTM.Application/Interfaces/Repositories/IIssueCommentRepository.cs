using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IIssueCommentRepository
{
    Task AddAsync(IssueComment comment, CancellationToken cancellationToken = default);

    Task<IssueComment?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid issueId, Guid commentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<IssueComment>> GetByIssueAsync(Guid workspaceId, Guid projectId, Guid issueId, CancellationToken cancellationToken = default);

    Task UpdateAsync(IssueComment comment, CancellationToken cancellationToken = default);

    Task DeleteAsync(IssueComment comment, CancellationToken cancellationToken = default);
}
