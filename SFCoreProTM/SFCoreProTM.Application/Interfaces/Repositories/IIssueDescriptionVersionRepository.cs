using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IIssueDescriptionVersionRepository
{
    Task AddAsync(IssueDescriptionVersion version, CancellationToken cancellationToken = default);
}
