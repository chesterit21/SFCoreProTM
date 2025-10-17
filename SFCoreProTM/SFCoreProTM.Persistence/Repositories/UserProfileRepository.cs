using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _context;

    public UserProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task AddAsync(UserProfile profile, CancellationToken cancellationToken)
    {
        return _context.AddAsync(profile, cancellationToken).AsTask();
    }
}

