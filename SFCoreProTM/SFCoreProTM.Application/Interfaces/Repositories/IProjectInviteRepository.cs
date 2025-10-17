using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IProjectInviteRepository
{
    Task<ProjectMemberInvite?> GetByTokenAsync(Guid token, CancellationToken cancellationToken);
    Task AddAsync(ProjectMemberInvite invite, CancellationToken cancellationToken);
    Task AddMemberAsync(ProjectMember member, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<ProjectMemberInvite>> GetByProjectAsync(Guid workspaceId, Guid projectId, CancellationToken cancellationToken);
}
