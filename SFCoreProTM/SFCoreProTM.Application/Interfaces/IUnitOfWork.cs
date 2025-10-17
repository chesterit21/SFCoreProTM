using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
