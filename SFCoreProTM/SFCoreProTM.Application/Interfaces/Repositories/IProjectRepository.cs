using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IProjectRepository
{
    Task AddAsync(
        Project project,
        IEnumerable<State> states,
        IEnumerable<ProjectMember> members,
        IEnumerable<IssueUserProperty> issueUserProperties,
        ProjectIdentifier identifier,
        CancellationToken cancellationToken = default);
}
