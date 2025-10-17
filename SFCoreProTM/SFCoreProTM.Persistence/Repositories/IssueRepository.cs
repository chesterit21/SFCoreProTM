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

public sealed class IssueRepository : IIssueRepository
{
    private const double DefaultSortOrder = 65_535d;
    private const double SortOrderIncrement = 10_000d;

    private readonly ApplicationDbContext _context;

    public IssueRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsByExternalIdAsync(
        Guid workspaceId,
        Guid projectId,
        string externalSource,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        return _context.Issues
            .AsNoTracking()
            .Where(issue => issue.WorkspaceId == workspaceId && issue.ProjectId == projectId)
            .AnyAsync(
                issue => issue.ExternalReference != null &&
                         issue.ExternalReference.Source == externalSource &&
                         issue.ExternalReference.Identifier == externalId,
                cancellationToken);
    }

    public async Task<Guid?> GetIdByExternalIdAsync(
        Guid workspaceId,
        Guid projectId,
        string externalSource,
        string externalId,
        CancellationToken cancellationToken = default)
    {
        var identifier = await _context.Issues
            .AsNoTracking()
            .Where(issue => issue.WorkspaceId == workspaceId && issue.ProjectId == projectId)
            .Where(issue => issue.ExternalReference != null &&
                            issue.ExternalReference.Source == externalSource &&
                            issue.ExternalReference.Identifier == externalId)
            .Select(issue => (Guid?)issue.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return identifier;
    }

    public Task<bool> ExistsAsync(Guid workspaceId, Guid projectId, Guid issueId, CancellationToken cancellationToken = default)
    {
        return _context.Issues
            .AsNoTracking()
            .AnyAsync(issue => issue.WorkspaceId == workspaceId && issue.ProjectId == projectId && issue.Id == issueId, cancellationToken);
    }

    public async Task AddAsync(Issue issue, IEnumerable<Guid> assigneeIds, IEnumerable<Guid> labelIds, CancellationToken cancellationToken = default)
    {
        await _context.Issues.AddAsync(issue, cancellationToken);

        foreach (var assigneeId in assigneeIds.Distinct())
        {
            var issueAssignee = IssueAssignee.Create(Guid.NewGuid(), issue.WorkspaceId, issue.ProjectId, issue.Id, assigneeId);
            await _context.IssueAssignees.AddAsync(issueAssignee, cancellationToken);
        }

        foreach (var labelId in labelIds.Distinct())
        {
            var issueLabel = IssueLabel.Create(Guid.NewGuid(), issue.WorkspaceId, issue.ProjectId, issue.Id, labelId);
            await _context.IssueLabels.AddAsync(issueLabel, cancellationToken);
        }
    }

    public async Task<int> GetNextSequenceIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var maxSequence = await _context.Issues
            .AsNoTracking()
            .Where(issue => issue.ProjectId == projectId)
            .MaxAsync(issue => (int?)issue.SequenceId, cancellationToken);

        return (maxSequence ?? 0) + 1;
    }

    public async Task<double> GetNextSortOrderAsync(Guid projectId, Guid? stateId, CancellationToken cancellationToken = default)
    {
        IQueryable<Issue> query = _context.Issues.AsNoTracking().Where(issue => issue.ProjectId == projectId);

        if (stateId.HasValue)
        {
            query = query.Where(issue => issue.StateId == stateId);
        }

        var maxSortOrder = await query.MaxAsync(issue => (double?)issue.SortOrder, cancellationToken);

        if (maxSortOrder is null)
        {
            return DefaultSortOrder;
        }

        return maxSortOrder.Value + SortOrderIncrement;
    }

    public async Task<Issue?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid issueId, CancellationToken cancellationToken = default)
    {
        var issue = await _context.Issues
            .FirstOrDefaultAsync(issue => issue.WorkspaceId == workspaceId && issue.ProjectId == projectId && issue.Id == issueId, cancellationToken);

        if (issue is null)
        {
            return null;
        }

        var assigneeIds = await _context.IssueAssignees
            .AsNoTracking()
            .Where(assignee => assignee.IssueId == issue.Id)
            .Select(assignee => assignee.AssigneeId)
            .ToListAsync(cancellationToken);

        var labelIds = await _context.IssueLabels
            .AsNoTracking()
            .Where(label => label.IssueId == issue.Id)
            .Select(label => label.LabelId)
            .ToListAsync(cancellationToken);

        issue.ReplaceAssignees(assigneeIds);
        issue.ReplaceLabels(labelIds);

        return issue;
    }

    public async Task UpdateAsync(Issue issue, IEnumerable<Guid> assigneeIds, IEnumerable<Guid> labelIds, CancellationToken cancellationToken = default)
    {
        _context.Issues.Update(issue);

        await _context.IssueAssignees
            .Where(assignee => assignee.IssueId == issue.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.IssueLabels
            .Where(label => label.IssueId == issue.Id)
            .ExecuteDeleteAsync(cancellationToken);

        foreach (var assigneeId in assigneeIds.Distinct())
        {
            var issueAssignee = IssueAssignee.Create(Guid.NewGuid(), issue.WorkspaceId, issue.ProjectId, issue.Id, assigneeId);
            await _context.IssueAssignees.AddAsync(issueAssignee, cancellationToken);
        }

        foreach (var labelId in labelIds.Distinct())
        {
            var issueLabel = IssueLabel.Create(Guid.NewGuid(), issue.WorkspaceId, issue.ProjectId, issue.Id, labelId);
            await _context.IssueLabels.AddAsync(issueLabel, cancellationToken);
        }
    }

    public async Task DeleteAsync(Issue issue, CancellationToken cancellationToken = default)
    {
        _context.Issues.Update(issue);

        await _context.IssueAssignees
            .Where(assignee => assignee.IssueId == issue.Id)
            .ExecuteDeleteAsync(cancellationToken);

        await _context.IssueLabels
            .Where(label => label.IssueId == issue.Id)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<bool> HasIssuesInStateAsync(Guid workspaceId, Guid projectId, Guid stateId, CancellationToken cancellationToken = default)
    {
        return _context.Issues
            .AsNoTracking()
            .AnyAsync(issue => issue.WorkspaceId == workspaceId && issue.ProjectId == projectId && issue.StateId == stateId, cancellationToken);
    }
}
