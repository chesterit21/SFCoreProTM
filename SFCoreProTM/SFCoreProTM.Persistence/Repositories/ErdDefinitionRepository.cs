using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public class ErdDefinitionRepository : IErdDefinitionRepository
{
    private readonly ApplicationDbContext _context;

    public ErdDefinitionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErdDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ErdDefinitions
            .Include(e => e.Attributes)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ErdDefinition>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.ErdDefinitions
            .Include(e => e.Attributes)
            .Where(e => e.ModuleId == moduleId)
            .OrderBy(e => e.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task AddAsync(ErdDefinition erdDefinition, CancellationToken cancellationToken = default)
    {
        await _context.ErdDefinitions.AddAsync(erdDefinition, cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateAsync(ErdDefinition erdDefinition, CancellationToken cancellationToken = default)
    {
        _context.ErdDefinitions.Update(erdDefinition);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task DeleteAsync(ErdDefinition erdDefinition, CancellationToken cancellationToken = default)
    {
        _context.ErdDefinitions.Remove(erdDefinition);
        await System.Threading.Tasks.Task.CompletedTask;
    }
}
