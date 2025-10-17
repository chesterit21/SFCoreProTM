using System;
using System.Threading;
using System.Threading.Tasks;
using SFCoreProTM.Domain.Entities.Users;

namespace SFCoreProTM.Application.Interfaces.Repositories;

public interface IUserProfileRepository
{
    Task AddAsync(UserProfile profile, CancellationToken cancellationToken);
}

