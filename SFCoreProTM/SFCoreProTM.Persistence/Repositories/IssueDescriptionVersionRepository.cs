using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class IssueDescriptionVersionRepository : IIssueDescriptionVersionRepository
{
    private readonly ApplicationDbContext _context;

    public IssueDescriptionVersionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(IssueDescriptionVersion version, CancellationToken cancellationToken = default)
    {
        return _context.IssueDescriptionVersions.AddAsync(version, cancellationToken).AsTask();
    }
}
