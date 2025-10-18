using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        Project project,
        IEnumerable<State> states,
        IEnumerable<ProjectMember> members,
        IEnumerable<IssueUserProperty> issueUserProperties,
        ProjectIdentifier identifier,
        CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        await _context.ProjectIdentifiers.AddAsync(identifier, cancellationToken);
        await _context.States.AddRangeAsync(states, cancellationToken);
        await _context.ProjectMembers.AddRangeAsync(members, cancellationToken);
        await _context.IssueUserProperties.AddRangeAsync(issueUserProperties, cancellationToken);
    }

    public Task<IReadOnlyCollection<Project>> ListByWorkspaceAsync(Guid workspaceId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
