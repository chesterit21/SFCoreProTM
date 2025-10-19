using System;
using System.Collections.Generic;
using System.Threading;
using Tasks = System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories
{
    public interface IModuleRepository
    {
        Tasks.Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Tasks.Task<IEnumerable<Module>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
        Tasks.Task AddAsync(Module module, CancellationToken cancellationToken = default);
        Tasks.Task UpdateAsync(Module module, CancellationToken cancellationToken = default);
        Tasks.Task DeleteAsync(Module module, CancellationToken cancellationToken = default);
    }
}