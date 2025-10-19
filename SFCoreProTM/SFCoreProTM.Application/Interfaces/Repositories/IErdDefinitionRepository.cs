using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IErdDefinitionRepository
{
    System.Threading.Tasks.Task<ErdDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task<IEnumerable<ErdDefinition>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task AddAsync(ErdDefinition erdDefinition, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task UpdateAsync(ErdDefinition erdDefinition, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteAsync(ErdDefinition erdDefinition, CancellationToken cancellationToken = default);
}