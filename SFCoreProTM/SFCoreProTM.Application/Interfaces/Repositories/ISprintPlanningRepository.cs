using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface ISprintPlanningRepository
{
    System.Threading.Tasks.Task<SprintPlanning?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<SprintPlanning>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<SprintPlanning>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task AddAsync(SprintPlanning sprintPlanning, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task UpdateAsync(SprintPlanning sprintPlanning, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteAsync(SprintPlanning sprintPlanning, CancellationToken cancellationToken = default);
}