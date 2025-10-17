using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class WorkspaceReadService : IWorkspaceReadService
{
    private readonly ApplicationDbContext _context;

    public WorkspaceReadService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> WorkspaceExistsAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        return _context.Workspaces
            .AsNoTracking()
            .AnyAsync(workspace => workspace.Id == workspaceId, cancellationToken);
    }

    public Task<bool> IsMemberAsync(Guid workspaceId, Guid memberId, CancellationToken cancellationToken = default)
    {
        return _context.WorkspaceMembers
            .AsNoTracking()
            .AnyAsync(member => member.WorkspaceId == workspaceId && member.MemberId == memberId && member.IsActive, cancellationToken);
    }

    public Task<bool> IsAdminAsync(Guid workspaceId, Guid memberId, CancellationToken cancellationToken = default)
    {
        return _context.WorkspaceMembers
            .AsNoTracking()
            .AnyAsync(member => member.WorkspaceId == workspaceId && member.MemberId == memberId && member.IsActive && (int)member.Role >= 2, cancellationToken);
    }

    public Task<bool> ProjectNameExistsAsync(Guid workspaceId, string name, CancellationToken cancellationToken = default)
    {
        return _context.Projects
            .AsNoTracking()
            .AnyAsync(project => project.WorkspaceId == workspaceId && project.Name == name, cancellationToken);
    }

    public Task<bool> ProjectIdentifierExistsAsync(Guid workspaceId, string identifier, CancellationToken cancellationToken = default)
    {
        return _context.ProjectIdentifiers
            .AsNoTracking()
            .AnyAsync(projectIdentifier => projectIdentifier.WorkspaceId == workspaceId && projectIdentifier.Name == identifier, cancellationToken);
    }
}
