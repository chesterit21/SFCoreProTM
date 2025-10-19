using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IFlowTaskRepository
{
    System.Threading.Tasks.Task<FlowTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<FlowTask>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task AddAsync(FlowTask flowTask, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task UpdateAsync(FlowTask flowTask, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteAsync(FlowTask flowTask, CancellationToken cancellationToken = default);
}