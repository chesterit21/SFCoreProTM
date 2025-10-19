using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<TaskEntity>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
        {
            return await _context.Tasks
                .Where(t => t.ModuleId == moduleId)
                .OrderBy(t => t.SortOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            await _context.Tasks.AddAsync(task, cancellationToken);
        }

        public async Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            _context.Tasks.Update(task);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(TaskEntity task, CancellationToken cancellationToken = default)
        {
            _context.Tasks.Remove(task);
            await Task.CompletedTask;
        }
    }
}