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

namespace SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueComments;

public sealed class GetIssueCommentsQueryHandler : IRequestHandler<GetIssueCommentsQuery, IReadOnlyCollection<IssueCommentDto>>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IIssueCommentRepository _issueCommentRepository;
    private readonly IMapper _mapper;

    public GetIssueCommentsQueryHandler(IWorkItemReadService workItemReadService, IIssueCommentRepository issueCommentRepository, IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _issueCommentRepository = issueCommentRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<IssueCommentDto>> Handle(GetIssueCommentsQuery request, CancellationToken cancellationToken)
    {
        var projectContext = await _workItemReadService.GetProjectContextAsync(request.ProjectId, cancellationToken);
        if (projectContext is null)
        {
            throw new NotFoundException($"Project '{request.ProjectId}' was not found.");
        }

        var comments = await _issueCommentRepository.GetByIssueAsync(projectContext.WorkspaceId, request.ProjectId, request.IssueId, cancellationToken);

        return comments.Select(c => _mapper.Map<IssueCommentDto>(c)).ToList();
    }
}
