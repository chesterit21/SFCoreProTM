using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces;

public interface IWorkspaceReadService
{
    Task<bool> WorkspaceExistsAsync(Guid workspaceId, CancellationToken cancellationToken = default);

    Task<bool> IsMemberAsync(Guid workspaceId, Guid memberId, CancellationToken cancellationToken = default);

    Task<bool> IsAdminAsync(Guid workspaceId, Guid memberId, CancellationToken cancellationToken = default);

    Task<bool> ProjectNameExistsAsync(Guid workspaceId, string name, CancellationToken cancellationToken = default);

    Task<bool> ProjectIdentifierExistsAsync(Guid workspaceId, string identifier, CancellationToken cancellationToken = default);
}
