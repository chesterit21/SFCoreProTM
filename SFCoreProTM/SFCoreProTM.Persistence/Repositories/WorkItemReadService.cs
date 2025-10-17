using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class WorkItemReadService : IWorkItemReadService
{
    private readonly ApplicationDbContext _context;

    public WorkItemReadService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectContext?> GetProjectContextAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var projection = await _context.Projects
            .AsNoTracking()
            .Where(project => project.Id == projectId)
            .Select(project => new ProjectContext(project.Id, project.WorkspaceId, project.DefaultAssigneeId))
            .FirstOrDefaultAsync(cancellationToken);

        return projection;
    }

    public Task<bool> StateExistsAsync(Guid projectId, Guid stateId, CancellationToken cancellationToken = default)
    {
        return _context.States
            .AsNoTracking()
            .AnyAsync(state => state.ProjectId == projectId && state.Id == stateId, cancellationToken);
    }

    public Task<StateSummary?> GetStateSummaryAsync(Guid projectId, Guid workspaceId, Guid stateId, CancellationToken cancellationToken = default)
    {
        return _context.States
            .AsNoTracking()
            .Where(state => state.ProjectId == projectId && state.WorkspaceId == workspaceId && state.Id == stateId)
            .Select(state => new StateSummary(state.Id, state.Group, state.IsTriage, state.IsDefault))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<StateSummary?> GetInitialStateAsync(Guid projectId, Guid workspaceId, CancellationToken cancellationToken = default)
    {
        var defaultState = await _context.States
            .AsNoTracking()
            .Where(state => state.ProjectId == projectId && state.WorkspaceId == workspaceId)
            .Where(state => !state.IsTriage && state.IsDefault)
            .OrderBy(state => state.Sequence)
            .Select(state => new StateSummary(state.Id, state.Group, state.IsTriage, state.IsDefault))
            .FirstOrDefaultAsync(cancellationToken);

        if (defaultState is not null)
        {
            return defaultState;
        }

        var firstNonTriage = await _context.States
            .AsNoTracking()
            .Where(state => state.ProjectId == projectId && state.WorkspaceId == workspaceId)
            .Where(state => !state.IsTriage)
            .OrderBy(state => state.Sequence)
            .Select(state => new StateSummary(state.Id, state.Group, state.IsTriage, state.IsDefault))
            .FirstOrDefaultAsync(cancellationToken);

        if (firstNonTriage is not null)
        {
            return firstNonTriage;
        }

        return await _context.States
            .AsNoTracking()
            .Where(state => state.ProjectId == projectId && state.WorkspaceId == workspaceId)
            .OrderBy(state => state.Sequence)
            .Select(state => new StateSummary(state.Id, state.Group, state.IsTriage, state.IsDefault))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IssueAssociations> GetIssueAssociationsAsync(Guid projectId, Guid workspaceId, Guid issueId, CancellationToken cancellationToken = default)
    {
        var moduleIds = await _context.Set<ModuleIssue>()
            .AsNoTracking()
            .Where(link => link.ProjectId == projectId && link.WorkspaceId == workspaceId && link.IssueId == issueId)
            .Select(link => link.ModuleId)
            .ToListAsync(cancellationToken);

        var cycleId = await _context.Set<CycleIssue>()
            .AsNoTracking()
            .Where(link => link.ProjectId == projectId && link.WorkspaceId == workspaceId && link.IssueId == issueId)
            .Select(link => (Guid?)link.CycleId)
            .FirstOrDefaultAsync(cancellationToken);

        return new IssueAssociations(moduleIds, cycleId);
    }

    public Task<bool> IssueExistsAsync(Guid projectId, Guid workspaceId, Guid issueId, CancellationToken cancellationToken = default)
    {
        return _context.Issues
            .AsNoTracking()
            .AnyAsync(issue => issue.ProjectId == projectId && issue.WorkspaceId == workspaceId && issue.Id == issueId, cancellationToken);
    }

    public Task<bool> EstimatePointExistsAsync(Guid projectId, Guid workspaceId, Guid estimatePointId, CancellationToken cancellationToken = default)
    {
        return _context.EstimatePoints
            .AsNoTracking()
            .AnyAsync(point => point.ProjectId == projectId && point.WorkspaceId == workspaceId && point.Id == estimatePointId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Guid>> FilterAssignableMemberIdsAsync(
        Guid projectId,
        IEnumerable<Guid> candidateMemberIds,
        CancellationToken cancellationToken = default)
    {
        var candidateIds = candidateMemberIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray() ?? Array.Empty<Guid>();

        if (candidateIds.Length == 0)
        {
            return Array.Empty<Guid>();
        }

        var validMembers = await _context.ProjectMembers
            .AsNoTracking()
            .Where(member =>
                member.ProjectId == projectId &&
                member.IsActive &&
                member.Role >= ProjectRole.Member &&
                candidateIds.Contains(member.MemberId))
            .Select(member => member.MemberId)
            .ToListAsync(cancellationToken);

        return validMembers;
    }

    public async Task<IReadOnlyCollection<Guid>> FilterLabelIdsAsync(
        Guid projectId,
        Guid workspaceId,
        IEnumerable<Guid> candidateLabelIds,
        CancellationToken cancellationToken = default)
    {
        var candidateIds = candidateLabelIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray() ?? Array.Empty<Guid>();

        if (candidateIds.Length == 0)
        {
            return Array.Empty<Guid>();
        }

        var validLabels = await _context.Labels
            .AsNoTracking()
            .Where(label => label.WorkspaceId == workspaceId && candidateIds.Contains(label.Id))
            .Select(label => label.Id)
            .ToListAsync(cancellationToken);

        return validLabels;
    }

    public async Task<Guid?> GetDefaultIssueTypeIdAsync(Guid projectId, Guid workspaceId, CancellationToken cancellationToken = default)
    {
        var projectDefault = await _context.ProjectIssueTypes
            .AsNoTracking()
            .Where(pit => pit.ProjectId == projectId && pit.IsDefault)
            .Select(pit => (Guid?)pit.IssueTypeId)
            .FirstOrDefaultAsync(cancellationToken);

        if (projectDefault.HasValue)
        {
            return projectDefault.Value;
        }

        return await _context.IssueTypes
            .AsNoTracking()
            .Where(type => type.WorkspaceId == workspaceId && type.IsDefault)
            .Select(type => (Guid?)type.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> StateNameExistsAsync(Guid projectId, Guid workspaceId, string name, Guid? excludeStateId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.States
            .AsNoTracking()
            .Where(state => state.ProjectId == projectId && state.WorkspaceId == workspaceId && state.Name == name);

        if (excludeStateId.HasValue)
        {
            query = query.Where(state => state.Id != excludeStateId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> StateExternalIdExistsAsync(Guid projectId, Guid workspaceId, string externalSource, string externalId, Guid? excludeStateId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.States
            .AsNoTracking()
            .Where(state => state.ProjectId == projectId && state.WorkspaceId == workspaceId && state.ExternalReference != null)
            .Where(state => state.ExternalReference!.Source == externalSource && state.ExternalReference.Identifier == externalId);

        if (excludeStateId.HasValue)
        {
            query = query.Where(state => state.Id != excludeStateId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> LabelNameExistsAsync(Guid projectId, Guid workspaceId, string name, Guid? excludeLabelId = null, CancellationToken cancellationToken = default)
    {
        _ = projectId;

        var query = _context.Labels
            .AsNoTracking()
            .Where(label => label.WorkspaceId == workspaceId && label.Name == name);

        if (excludeLabelId.HasValue)
        {
            query = query.Where(label => label.Id != excludeLabelId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> LabelExternalIdExistsAsync(Guid projectId, Guid workspaceId, string externalSource, string externalId, Guid? excludeLabelId = null, CancellationToken cancellationToken = default)
    {
        _ = projectId;

        var query = _context.Labels
            .AsNoTracking()
            .Where(label => label.WorkspaceId == workspaceId && label.ExternalReference != null)
            .Where(label => label.ExternalReference!.Source == externalSource && label.ExternalReference.Identifier == externalId);

        if (excludeLabelId.HasValue)
        {
            query = query.Where(label => label.Id != excludeLabelId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> IsProjectMemberAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.ProjectMembers
            .AsNoTracking()
            .AnyAsync(member => member.ProjectId == projectId && member.MemberId == userId && member.IsActive, cancellationToken);
    }

    public Task<bool> IsProjectAdminAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.ProjectMembers
            .AsNoTracking()
            .AnyAsync(member => member.ProjectId == projectId && member.MemberId == userId && member.IsActive && (int)member.Role >= 20, cancellationToken);
    }
}
