using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.SprintPlannings;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.SprintPlannings.Queries.GetSprintPlanningsByModuleId;

public class GetSprintPlanningsByModuleIdQueryHandler : IRequestHandler<GetSprintPlanningsByModuleIdQuery, IEnumerable<SprintPlanningDto>>
{
    private readonly ISprintPlanningRepository _sprintPlanningRepository;

    public GetSprintPlanningsByModuleIdQueryHandler(ISprintPlanningRepository sprintPlanningRepository)
    {
        _sprintPlanningRepository = sprintPlanningRepository;
    }

    public async Task<IEnumerable<SprintPlanningDto>> Handle(GetSprintPlanningsByModuleIdQuery request, CancellationToken cancellationToken)
    {
        var sprintPlannings = await _sprintPlanningRepository.GetByModuleIdAsync(request.ModuleId, cancellationToken);
        
        return sprintPlannings.Select(sprintPlanning => new SprintPlanningDto
        {
            Id = sprintPlanning.Id,
            WorkspaceId = sprintPlanning.WorkspaceId,
            ProjectId = sprintPlanning.ProjectId,
            ModuleId = sprintPlanning.ModuleId,
            TaskId = sprintPlanning.TaskId,
            Name = sprintPlanning.Name,
            Description = sprintPlanning.Description,
            StartDate = sprintPlanning.StartDate,
            TargetDate = sprintPlanning.TargetDate,
            SortOrder = sprintPlanning.SortOrder,
            SprintStatus = (int)sprintPlanning.SprintStatus,
            Note = sprintPlanning.Note
        }).ToList();
    }
}