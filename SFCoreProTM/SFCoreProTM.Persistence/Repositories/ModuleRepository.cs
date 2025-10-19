using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly ApplicationDbContext _context;

        public ModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Module>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
        {
            return await _context.Modules
                .Where(m => m.ProjectId == projectId)
                .OrderBy(m => m.SortOrder)
                .ToListAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task AddAsync(Module module, CancellationToken cancellationToken = default)
        {
            await _context.Modules.AddAsync(module, cancellationToken);
        }

        public async System.Threading.Tasks.Task UpdateAsync(Module module, CancellationToken cancellationToken = default)
        {
            _context.Modules.Update(module);
            await System.Threading.Tasks.Task.CompletedTask;
        }

        public async System.Threading.Tasks.Task DeleteAsync(Module module, CancellationToken cancellationToken = default)
        {
            _context.Modules.Remove(module);
            await System.Threading.Tasks.Task.CompletedTask;
        }
    }
}