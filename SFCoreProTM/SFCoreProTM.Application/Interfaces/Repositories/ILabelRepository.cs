using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Workspaces;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface ILabelRepository
{
    Task AddAsync(Label label, CancellationToken cancellationToken = default);

    Task<Label?> GetByIdAsync(Guid workspaceId, Guid projectId, Guid labelId, CancellationToken cancellationToken = default);

    Task UpdateAsync(Label label, CancellationToken cancellationToken = default);

    Task DeleteAsync(Label label, CancellationToken cancellationToken = default);
}
