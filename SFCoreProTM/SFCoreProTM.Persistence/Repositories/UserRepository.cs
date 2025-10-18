using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.ValueObjects;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(email);

        var providerEmail = ValueConverters.EmailAddressConverter.ConvertToProvider(email);

        return _context.Users
            .FromSqlInterpolated($"SELECT * FROM users WHERE email = {providerEmail} LIMIT 1")
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return _context.Users.SingleOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);
        return _context.Users.AddAsync(user, cancellationToken).AsTask();
    }
}
