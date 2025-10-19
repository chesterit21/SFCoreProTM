using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Workspaces;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Workspace entity, CancellationToken cancellationToken);
}