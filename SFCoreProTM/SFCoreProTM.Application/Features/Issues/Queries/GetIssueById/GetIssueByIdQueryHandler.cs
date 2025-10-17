using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Issues;
using SFCoreProTM.Application.Exceptions;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Issues.Queries.GetIssueById;

public sealed class GetIssueByIdQueryHandler : IRequestHandler<GetIssueByIdQuery, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IWorkItemReadService _workItemReadService;
    private readonly IMapper _mapper;

    public GetIssueByIdQueryHandler(IIssueRepository issueRepository, IWorkItemReadService workItemReadService, IMapper mapper)
    {
        _issueRepository = issueRepository;
        _workItemReadService = workItemReadService;
        _mapper = mapper;
    }

    public async Task<IssueDto> Handle(GetIssueByIdQuery request, CancellationToken cancellationToken)
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

        return _mapper.Map<IssueDto>(issue);
    }
}
