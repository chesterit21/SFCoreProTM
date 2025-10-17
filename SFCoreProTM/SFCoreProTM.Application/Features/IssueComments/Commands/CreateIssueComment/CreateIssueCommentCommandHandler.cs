using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.CreateIssueComment;

public sealed class CreateIssueCommentCommandHandler : IRequestHandler<CreateIssueCommentCommand, IssueCommentDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IIssueCommentRepository _issueCommentRepository;
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIssueHistoryService _issueHistoryService;
    private readonly IMapper _mapper;

    public CreateIssueCommentCommandHandler(
        IIssueRepository issueRepository,
        IIssueCommentRepository issueCommentRepository,
        IWorkItemReadService workItemReadService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IIssueHistoryService issueHistoryService,
        IMapper mapper)
    {
        _issueRepository = issueRepository;
        _issueCommentRepository = issueCommentRepository;
        _workItemReadService = workItemReadService;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _issueHistoryService = issueHistoryService;
        _mapper = mapper;
    }

    public async Task<IssueCommentDto> Handle(CreateIssueCommentCommand request, CancellationToken cancellationToken)
    {
        var payload = request.Payload;

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

        var commentRichText = RichTextContent.Create(
            payload.CommentPlainText,
            payload.CommentHtml,
            payload.CommentBinary,
            payload.CommentJson);

        var attachments = new List<Url>();
        foreach (var attachment in payload.Attachments)
        {
            attachments.Add(Url.Create(attachment));
        }

        var comment = IssueComment.Create(
            Guid.NewGuid(),
            projectContext.WorkspaceId,
            request.ProjectId,
            request.IssueId,
            payload.ActorId,
            commentRichText,
            payload.Access);

        comment.UpdateComment(
            commentRichText,
            payload.CommentPlainText,
            StructuredData.FromJson(payload.CommentJson));

        comment.ReplaceAttachments(attachments);
        var now = _dateTimeProvider.UtcNow;
        comment.SetAuditTrail(AuditTrail.Create(now, payload.ActorId, now, payload.ActorId, null));

        comment.SetExternalReference(payload.ExternalSource, payload.ExternalId);
        comment.SetEditedAt(null);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _issueCommentRepository.AddAsync(comment, cancellationToken);
        await _issueHistoryService.LogAsync(issue, payload.ActorId, now, includeDescription: false, changeSource: "comment:create", cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<IssueCommentDto>(comment);
    }
}
