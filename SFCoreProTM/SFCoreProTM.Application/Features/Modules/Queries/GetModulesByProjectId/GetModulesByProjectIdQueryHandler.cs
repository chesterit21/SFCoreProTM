using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Modules;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Modules.Queries.GetModulesByProjectId;

public class GetModulesByProjectIdQueryHandler : IRequestHandler<GetModulesByProjectIdQuery, IEnumerable<ModuleDto>>
{
    private readonly IModuleRepository _moduleRepository;

    public GetModulesByProjectIdQueryHandler(IModuleRepository moduleRepository)
    {
        _moduleRepository = moduleRepository;
    }

    public async Task<IEnumerable<ModuleDto>> Handle(GetModulesByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var modules = await _moduleRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        
        return modules.Select(module => new ModuleDto
        {
            Id = module.Id,
            WorkspaceId = module.WorkspaceId,
            ProjectId = module.ProjectId,
            Name = module.Name,
            Description = module.Description,
            SortOrder = module.SortOrder,
            Status = (int)module.Status
        }).ToList();
    }
}