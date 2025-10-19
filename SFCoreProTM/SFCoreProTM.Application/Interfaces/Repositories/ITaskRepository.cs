using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Application.Interfaces.Repositories
{
    public interface ITaskRepository
    {
        Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskEntity>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
        Task AddAsync(TaskEntity task, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
        Task DeleteAsync(TaskEntity task, CancellationToken cancellationToken = default);
    }
}