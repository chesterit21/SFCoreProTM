using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Issues.Commands.DeleteIssue;

public sealed class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIssueHistoryService _issueHistoryService;

    public DeleteIssueCommandHandler(
        IIssueRepository issueRepository,
        IWorkItemReadService workItemReadService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IIssueHistoryService issueHistoryService)
    {
        _issueRepository = issueRepository;
        _workItemReadService = workItemReadService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _issueHistoryService = issueHistoryService;
    }

    public async Task Handle(DeleteIssueCommand request, CancellationToken cancellationToken)
    {
        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found.");
        }

        var issue = await _issueRepository.GetByIdAsync(projectContext.WorkspaceId, request.ProjectId, request.IssueId, cancellationToken);
        if (issue is null)
        {
            throw new NotFoundException($"Issue '{request.IssueId}' was not found in project '{request.ProjectId}'.");
        }

        var timestamp = _dateTimeProvider.UtcNow;
        issue.SoftDelete(timestamp, request.ActorId);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _issueRepository.DeleteAsync(issue, cancellationToken);
        await _issueHistoryService.LogAsync(issue, request.ActorId, timestamp, includeDescription: true, changeSource: "issue:delete", cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
