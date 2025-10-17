using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class IssueCommentRepository : IIssueCommentRepository
{
    private readonly ApplicationDbContext _context;

    public IssueCommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(IssueComment comment, CancellationToken cancellationToken = default)
    {
        return _context.IssueComments.AddAsync(comment, cancellationToken).AsTask();
    }

    public Task<IssueComment?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid issueId, Guid commentId, CancellationToken cancellationToken = default)
    {
        return _context.IssueComments
            .FirstOrDefaultAsync(comment =>
                comment.WorkspaceId == workspaceId &&
                comment.ProjectId == projectId &&
                comment.IssueId == issueId &&
                comment.Id == commentId,
                cancellationToken);
    }

    public async Task<IReadOnlyCollection<IssueComment>> GetByIssueAsync(Guid workspaceId, Guid projectId, Guid issueId, CancellationToken cancellationToken = default)
    {
        var comments = await _context.IssueComments
            .AsNoTracking()
            .Where(comment => comment.WorkspaceId == workspaceId && comment.ProjectId == projectId && comment.IssueId == issueId)
            .OrderBy(comment => comment.AuditTrail.CreatedAt)
            .ToListAsync(cancellationToken);

        return comments;
    }

    public Task UpdateAsync(IssueComment comment, CancellationToken cancellationToken = default)
    {
        _context.IssueComments.Update(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IssueComment comment, CancellationToken cancellationToken = default)
    {
        _context.IssueComments.Remove(comment);
        return Task.CompletedTask;
    }
}
