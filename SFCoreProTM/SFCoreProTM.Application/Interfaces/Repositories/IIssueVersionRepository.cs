using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Issues;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IIssueVersionRepository
{
    Task AddAsync(IssueVersion version, CancellationToken cancellationToken = default);
}
