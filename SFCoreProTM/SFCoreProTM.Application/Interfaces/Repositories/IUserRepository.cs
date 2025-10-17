using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Users;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);
}
