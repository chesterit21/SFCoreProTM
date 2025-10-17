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
using SFCoreProTM.Domain.ValueObjects;

namespace SFCoreProTM.Application.Features.IssueComments.Commands.UpdateIssueComment;

public sealed class UpdateIssueCommentCommandHandler : IRequestHandler<UpdateIssueCommentCommand, IssueCommentDto>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IIssueCommentRepository _issueCommentRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIssueHistoryService _issueHistoryService;
    private readonly IMapper _mapper;

    public UpdateIssueCommentCommandHandler(
        IWorkItemReadService workItemReadService,
        IIssueCommentRepository issueCommentRepository,
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IIssueHistoryService issueHistoryService,
        IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _issueCommentRepository = issueCommentRepository;
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _issueHistoryService = issueHistoryService;
        _mapper = mapper;
    }

    public async Task<IssueCommentDto> Handle(UpdateIssueCommentCommand request, CancellationToken cancellationToken)
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

        var comment = await _issueCommentRepository.GetByIdAsync(projectContext.WorkspaceId, request.ProjectId, request.IssueId, request.CommentId, cancellationToken);
        if (comment is null)
        {
            throw new NotFoundException($"Comment '{request.CommentId}' was not found.");
        }

        if (payload.Access.HasValue)
        {
            comment.SetAccess(payload.Access.Value);
        }

        var newRichText = RichTextContent.Create(
            payload.CommentPlainText ?? comment.Comment.PlainText,
            payload.CommentHtml ?? comment.Comment.Html,
            payload.CommentBinary ?? comment.Comment.Binary,
            payload.CommentJson ?? comment.Comment.Json);

        comment.UpdateComment(
            newRichText,
            payload.CommentPlainText ?? comment.CommentStripped,
            StructuredData.FromJson(payload.CommentJson ?? comment.CommentJson.RawJson));

        if (payload.Attachments is not null)
        {
            var attachments = new List<Url>();
            foreach (var attachment in payload.Attachments)
            {
                attachments.Add(Url.Create(attachment));
            }

            comment.ReplaceAttachments(attachments);
        }

        comment.SetExternalReference(payload.ExternalSource ?? comment.ExternalReference?.Source, payload.ExternalId ?? comment.ExternalReference?.Identifier);
        var now = _dateTimeProvider.UtcNow;
        comment.SetEditedAt(now);

        var updatedAudit = AuditTrail.Create(
            comment.AuditTrail.CreatedAt,
            comment.AuditTrail.CreatedById,
            now,
            payload.ActorId,
            comment.AuditTrail.DeletedAt);
        comment.SetAuditTrail(updatedAudit);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        await _issueCommentRepository.UpdateAsync(comment, cancellationToken);
        await _issueHistoryService.LogAsync(issue, payload.ActorId, now, includeDescription: false, changeSource: "comment:update", cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return _mapper.Map<IssueCommentDto>(comment);
    }
}
