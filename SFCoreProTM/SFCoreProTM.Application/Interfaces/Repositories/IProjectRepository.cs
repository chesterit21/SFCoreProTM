using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyCollection<Project?>> ListByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default);
    
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Project project);
    
    void Update(Project project);
    
    Task<bool> NameExistsAsync(Guid workspaceId, string name, CancellationToken cancellationToken = default);
}