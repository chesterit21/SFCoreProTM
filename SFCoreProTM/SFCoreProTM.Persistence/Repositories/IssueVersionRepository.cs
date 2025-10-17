using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class IssueVersionRepository : IIssueVersionRepository
{
    private readonly ApplicationDbContext _context;

    public IssueVersionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(IssueVersion version, CancellationToken cancellationToken = default)
    {
        return _context.IssueVersions.AddAsync(version, cancellationToken).AsTask();
    }
}
