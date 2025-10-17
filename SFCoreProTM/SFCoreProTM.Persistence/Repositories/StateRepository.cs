using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class StateRepository : IStateRepository
{
    private readonly ApplicationDbContext _context;

    public StateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(State state, CancellationToken cancellationToken = default)
    {
        return _context.States.AddAsync(state, cancellationToken).AsTask();
    }

    public Task<State?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid stateId, CancellationToken cancellationToken = default)
    {
        return _context.States
            .FirstOrDefaultAsync(state => state.WorkspaceId == workspaceId && state.ProjectId == projectId && state.Id == stateId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<State>> GetByProjectAsync(Guid workspaceId, Guid projectId, CancellationToken cancellationToken = default)
    {
        var states = await _context.States
            .AsNoTracking()
            .Where(state => state.WorkspaceId == workspaceId && state.ProjectId == projectId)
            .OrderBy(state => state.Sequence)
            .ToListAsync(cancellationToken);

        return states;
    }

    public Task UpdateAsync(State state, CancellationToken cancellationToken = default)
    {
        _context.States.Update(state);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(State state, CancellationToken cancellationToken = default)
    {
        _context.States.Remove(state);
        return Task.CompletedTask;
    }

    public async Task SetDefaultAsync(Guid workspaceId, Guid projectId, Guid stateId, CancellationToken cancellationToken = default)
    {
        await _context.States
            .Where(state => state.WorkspaceId == workspaceId && state.ProjectId == projectId && state.Id != stateId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(state => state.IsDefault, _ => false), cancellationToken);

        await _context.States
            .Where(state => state.WorkspaceId == workspaceId && state.ProjectId == projectId && state.Id == stateId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(state => state.IsDefault, _ => true), cancellationToken);
    }
}
