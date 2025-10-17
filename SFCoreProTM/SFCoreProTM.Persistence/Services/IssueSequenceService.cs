using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Persistence.Data;

namespace SFCoreProTM.Persistence.Services;

public sealed class IssueSequenceService : IIssueSequenceService
{
    private readonly ApplicationDbContext _context;
    private readonly IIssueRepository _issueRepository;

    public IssueSequenceService(ApplicationDbContext context, IIssueRepository issueRepository)
    {
        _context = context;
        _issueRepository = issueRepository;
    }

    public async Task<IssueSequenceReservation> ReserveAsync(Guid projectId, Guid? stateId, CancellationToken cancellationToken = default)
    {
        await AcquireProjectLockAsync(projectId, cancellationToken);

        var nextSequenceId = await _issueRepository.GetNextSequenceIdAsync(projectId, cancellationToken);
        var nextSortOrder = await _issueRepository.GetNextSortOrderAsync(projectId, stateId, cancellationToken);

        return new IssueSequenceReservation(nextSequenceId, nextSortOrder);
    }

    public async Task<double> ReserveSortOrderAsync(Guid projectId, Guid? stateId, CancellationToken cancellationToken = default)
    {
        await AcquireProjectLockAsync(projectId, cancellationToken);
        return await _issueRepository.GetNextSortOrderAsync(projectId, stateId, cancellationToken);
    }

    private async Task AcquireProjectLockAsync(Guid projectId, CancellationToken cancellationToken)
    {
        await EnsureConnectionOpenAsync(cancellationToken);

        var lockKey = ConvertGuidToLockKey(projectId);
        await _context.Database.ExecuteSqlRawAsync("SELECT pg_advisory_xact_lock({0})", cancellationToken, lockKey);
    }

    private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync(cancellationToken);
        }
    }

    private static long ConvertGuidToLockKey(Guid value)
    {
        var bytes = value.ToByteArray();
        return BitConverter.ToInt64(bytes, 0);
    }
}
