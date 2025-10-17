using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IStateRepository
{
    Task AddAsync(State state, CancellationToken cancellationToken = default);

    Task<State?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid stateId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<State>> GetByProjectAsync(Guid workspaceId, Guid projectId, CancellationToken cancellationToken = default);

    Task UpdateAsync(State state, CancellationToken cancellationToken = default);

    Task DeleteAsync(State state, CancellationToken cancellationToken = default);

    Task SetDefaultAsync(Guid workspaceId, Guid projectId, Guid stateId, CancellationToken cancellationToken = default);
}
