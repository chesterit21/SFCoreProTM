using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces;

public sealed record ProjectContext(Guid ProjectId, Guid WorkspaceId);

public interface IWorkItemReadService
{
    Task<ProjectContext?> GetProjectContextAsync(Guid projectId, CancellationToken cancellationToken = default);
    
    // Methods for Module domain
    Task<bool> ModuleExistsAsync(Guid projectId, Guid moduleId, CancellationToken cancellationToken = default);
    
    // Methods that might be needed for Task domain
    Task<bool> IssueExistsAsync(Guid projectId, Guid issueId, CancellationToken cancellationToken = default);
}
