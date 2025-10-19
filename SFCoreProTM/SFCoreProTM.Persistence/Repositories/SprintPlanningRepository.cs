using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public class SprintPlanningRepository : ISprintPlanningRepository
{
    private readonly ApplicationDbContext _context;

    public SprintPlanningRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<SprintPlanning?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SprintPlannings
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task<IEnumerable<SprintPlanning>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.SprintPlannings
            .Where(s => s.ModuleId == moduleId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task<IEnumerable<SprintPlanning>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.SprintPlannings
            .Where(s => s.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task AddAsync(SprintPlanning sprintPlanning, CancellationToken cancellationToken = default)
    {
        await _context.SprintPlannings.AddAsync(sprintPlanning, cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateAsync(SprintPlanning sprintPlanning, CancellationToken cancellationToken = default)
    {
        _context.SprintPlannings.Update(sprintPlanning);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task DeleteAsync(SprintPlanning sprintPlanning, CancellationToken cancellationToken = default)
    {
        _context.SprintPlannings.Remove(sprintPlanning);
        await System.Threading.Tasks.Task.CompletedTask;
    }
}