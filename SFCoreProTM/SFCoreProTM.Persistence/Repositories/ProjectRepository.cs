using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Project project)
    {
        _context.Projects.Add(project);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public void Update(Project project)
    {
        _context.Projects.Update(project);
    }

    public async Task<IReadOnlyCollection<Project?>> ListByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return (await _context.Projects
            .AsNoTracking()
            .Where(project => project.WorkspaceId == workspaceId)
            .ToListAsync(cancellationToken))
            .AsReadOnly();
    }
    
    public Task<bool> NameExistsAsync(Guid workspaceId, string name, CancellationToken cancellationToken = default) =>
        _context.Projects
            .AsNoTracking()
            .AnyAsync(p => p.WorkspaceId == workspaceId && p.Name == name, cancellationToken);
}