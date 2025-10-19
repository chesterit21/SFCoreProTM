using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class WorkspaceRepository : IWorkspaceRepository
{
    private readonly ApplicationDbContext _context;

    public WorkspaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _context.Workspaces.SingleOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public Task AddAsync(Workspace entity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return _context.Workspaces.AddAsync(entity, cancellationToken).AsTask();
    }
}
