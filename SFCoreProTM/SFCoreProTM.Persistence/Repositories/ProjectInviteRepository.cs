using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class ProjectInviteRepository : IProjectInviteRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectInviteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<ProjectMemberInvite?> GetByTokenAsync(Guid token, CancellationToken cancellationToken)
    {
        return _context.Set<ProjectMemberInvite>()
            .AsTracking()
            .SingleOrDefaultAsync(i => i.Token == token, cancellationToken);
    }

    public Task AddAsync(ProjectMemberInvite invite, CancellationToken cancellationToken)
    {
        return _context.Set<ProjectMemberInvite>().AddAsync(invite, cancellationToken).AsTask();
    }

    public Task AddMemberAsync(ProjectMember member, CancellationToken cancellationToken)
    {
        return _context.Set<ProjectMember>().AddAsync(member, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<ProjectMemberInvite>> GetByProjectAsync(Guid workspaceId, Guid projectId, CancellationToken cancellationToken)
    {
        var items = await _context.Set<ProjectMemberInvite>()
            .AsNoTracking()
            .Where(i => i.WorkspaceId == workspaceId && i.ProjectId == projectId)
            .ToListAsync(cancellationToken);
        return items;
    }
}
