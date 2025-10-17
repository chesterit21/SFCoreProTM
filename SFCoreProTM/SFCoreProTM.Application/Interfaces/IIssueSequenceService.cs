using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFCoreProTM.Application.Interfaces;

public sealed record IssueSequenceReservation(int SequenceId, double SortOrder);

public interface IIssueSequenceService
{
    Task<IssueSequenceReservation> ReserveAsync(Guid projectId, Guid? stateId, CancellationToken cancellationToken = default);

    Task<double> ReserveSortOrderAsync(Guid projectId, Guid? stateId, CancellationToken cancellationToken = default);
}
