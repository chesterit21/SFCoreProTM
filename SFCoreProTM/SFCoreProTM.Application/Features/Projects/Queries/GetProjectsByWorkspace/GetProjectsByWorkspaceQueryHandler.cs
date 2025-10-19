using MediatR;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Projects.Queries.GetProjectsByWorkspace;

public sealed class GetProjectsByWorkspaceQueryHandler : IRequestHandler<GetProjectsByWorkspaceQuery, IReadOnlyCollection<ProjectSummaryDto>>
{
    private readonly IProjectRepository _projectRepository;

    public GetProjectsByWorkspaceQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyCollection<ProjectSummaryDto>> Handle(GetProjectsByWorkspaceQuery request, CancellationToken cancellationToken)
    {
        var result = await _projectRepository.ListByWorkspaceAsync(request.WorkspaceId);
        return result.Where(p => p != null)
                     .Select(p => new ProjectSummaryDto
                     {
                         Id = p!.Id,
                         Name = p.Name,
                         Description = p.Description
                     }).ToList();
    }
}