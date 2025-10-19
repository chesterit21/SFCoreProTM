using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public class FlowTaskRepository : IFlowTaskRepository
{
    private readonly ApplicationDbContext _context;

    public FlowTaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task<FlowTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.FlowTasks
            .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async System.Threading.Tasks.Task<IEnumerable<FlowTask>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.FlowTasks
            .Where(f => f.TaskId == taskId)
            .OrderBy(f => f.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task AddAsync(FlowTask flowTask, CancellationToken cancellationToken = default)
    {
        await _context.FlowTasks.AddAsync(flowTask, cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateAsync(FlowTask flowTask, CancellationToken cancellationToken = default)
    {
        _context.FlowTasks.Update(flowTask);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task DeleteAsync(FlowTask flowTask, CancellationToken cancellationToken = default)
    {
        _context.FlowTasks.Remove(flowTask);
        await System.Threading.Tasks.Task.CompletedTask;
    }
}