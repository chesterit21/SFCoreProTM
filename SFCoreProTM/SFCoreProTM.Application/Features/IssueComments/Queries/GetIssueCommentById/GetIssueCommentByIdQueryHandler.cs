using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.IssueComments.Queries.GetIssueCommentById;

public sealed class GetIssueCommentByIdQueryHandler : IRequestHandler<GetIssueCommentByIdQuery, IssueCommentDto>
{
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IIssueCommentRepository _issueCommentRepository;
    private readonly IMapper _mapper;

    public GetIssueCommentByIdQueryHandler(IWorkItemReadService workItemReadService, IIssueCommentRepository issueCommentRepository, IMapper mapper)
    {
        _workItemReadService = workItemReadService;
        _issueCommentRepository = issueCommentRepository;
        _mapper = mapper;
    }

    public async Task<IssueCommentDto> Handle(GetIssueCommentByIdQuery request, CancellationToken cancellationToken)
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

        return _mapper.Map<IssueCommentDto>(comment);
    }
}
