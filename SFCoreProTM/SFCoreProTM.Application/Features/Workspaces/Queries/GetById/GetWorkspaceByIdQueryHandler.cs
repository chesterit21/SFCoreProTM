using MediatR;
using SFCoreProTM.Application.DTOs.Workspaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Workspaces.Queries.GetById;

public sealed class GetWorkspaceByIdQueryHandler : IRequestHandler<GetWorkspaceByIdQuery, WorkspaceDto?>
{
    private readonly IWorkspaceRepository _workspaceRepository;

    public GetWorkspaceByIdQueryHandler(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<WorkspaceDto?> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceRepository.GetByIdAsync(request.WorkspaceId, cancellationToken);
        
        if (workspace is null)
        {
            return null;
        }

        return new WorkspaceDto
        {
            Id = workspace.Id,
            Name = workspace.Name,
            Logo = workspace.Logo?.Value,
            LogoAssetId = workspace.LogoAssetId,
            OwnerId = workspace.OwnerId,
            Slug = workspace.Slug.Value,
            OrganizationSize = workspace.OrganizationSize,
            Timezone = workspace.Timezone,
            BackgroundColor = workspace.BackgroundColor?.Value
        };
    }
}