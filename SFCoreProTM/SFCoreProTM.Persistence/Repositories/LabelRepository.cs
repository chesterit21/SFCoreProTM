using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Workspaces;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class LabelRepository : ILabelRepository
{
    private readonly ApplicationDbContext _context;

    public LabelRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(Label label, CancellationToken cancellationToken = default)
    {
        return _context.Labels.AddAsync(label, cancellationToken).AsTask();
    }

    public Task<Label?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid labelId, CancellationToken cancellationToken = default)
    {
        return _context.Labels
            .FirstOrDefaultAsync(label => label.WorkspaceId == workspaceId && label.Id == labelId, cancellationToken);
    }

    public Task UpdateAsync(Label label, CancellationToken cancellationToken = default)
    {
        _context.Labels.Update(label);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Label label, CancellationToken cancellationToken = default)
    {
        _context.Labels.Remove(label);
        return Task.CompletedTask;
    }
}
