using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.Interfaces;

public interface IIssueHistoryService
{
    Task LogAsync(Issue issue, Guid actorId, DateTime recordedAt, bool includeDescription, string? changeSource = null, CancellationToken cancellationToken = default);
}
