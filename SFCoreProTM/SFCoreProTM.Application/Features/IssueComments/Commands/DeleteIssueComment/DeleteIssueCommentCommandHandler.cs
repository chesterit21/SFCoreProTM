using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.DeleteIssueComment;

public sealed class DeleteIssueCommentCommandHandler : IRequestHandler<DeleteIssueCommentCommand, Unit>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IIssueCommentRepository _issueCommentRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIssueHistoryService _issueHistoryService;

    public DeleteIssueCommentCommandHandler(
        IWorkItemReadService workItemReadService,
        IIssueCommentRepository issueCommentRepository,
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IIssueHistoryService issueHistoryService)
    {
        _workItemReadService = workItemReadService;
        _issueCommentRepository = issueCommentRepository;
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _issueHistoryService = issueHistoryService;
    }

    public async Task<Unit> Handle(DeleteIssueCommentCommand request, CancellationToken cancellationToken)
    {
        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found.");
        }

        var comment = await _issueCommentRepository.GetByIdAsync(projectContext.WorkspaceId, request.ProjectId, request.IssueId, request.CommentId, cancellationToken);
        if (comment is null)
        {
            throw new NotFoundException($"Comment '{request.CommentId}' was not found.");
        }

        var issue = await _issueRepository.GetByIdAsync(projectContext.WorkspaceId, request.ProjectId, request.IssueId, cancellationToken);
        if (issue is null)
        {
            throw new NotFoundException($"Issue '{request.IssueId}' was not found in project '{request.ProjectId}'.");
        }

        var now = _dateTimeProvider.UtcNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _issueCommentRepository.DeleteAsync(comment, cancellationToken);
        await _issueHistoryService.LogAsync(issue, request.ActorId, now, includeDescription: false, changeSource: "comment:delete", cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Unit.Value;
    }
}
