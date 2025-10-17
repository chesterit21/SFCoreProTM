using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Workspaces;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IWorkspaceInviteRepository
{
    Task<WorkspaceMemberInvite?> GetByTokenAsync(Guid token, CancellationToken cancellationToken);
    Task AddAsync(WorkspaceMemberInvite invite, CancellationToken cancellationToken);
    Task AddMemberAsync(WorkspaceMember member, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<WorkspaceMemberInvite>> GetByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken);
}
