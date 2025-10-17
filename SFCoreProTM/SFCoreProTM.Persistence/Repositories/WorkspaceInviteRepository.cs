using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class WorkspaceInviteRepository : IWorkspaceInviteRepository
{
    private readonly ApplicationDbContext _context;

    public WorkspaceInviteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<WorkspaceMemberInvite?> GetByTokenAsync(Guid token, CancellationToken cancellationToken)
    {
        return _context.Set<WorkspaceMemberInvite>()
            .AsTracking()
            .SingleOrDefaultAsync(i => i.Token == token, cancellationToken);
    }

    public Task AddMemberAsync(WorkspaceMember member, CancellationToken cancellationToken)
    {
        return _context.Set<WorkspaceMember>().AddAsync(member, cancellationToken).AsTask();
    }

    public Task AddAsync(WorkspaceMemberInvite invite, CancellationToken cancellationToken)
    {
        return _context.Set<WorkspaceMemberInvite>().AddAsync(invite, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<WorkspaceMemberInvite>> GetByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken)
    {
        var items = await _context.Set<WorkspaceMemberInvite>()
            .AsNoTracking()
            .Where(i => i.WorkspaceId == workspaceId)
            .ToListAsync(cancellationToken);
        return items;
    }
}
