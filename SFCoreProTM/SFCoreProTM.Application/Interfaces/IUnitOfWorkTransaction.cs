using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces;

public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
