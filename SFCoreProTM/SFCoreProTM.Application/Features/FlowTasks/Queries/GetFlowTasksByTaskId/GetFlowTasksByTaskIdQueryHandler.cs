using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.FlowTasks;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.FlowTasks.Queries.GetFlowTasksByTaskId;

public class GetFlowTasksByTaskIdQueryHandler : IRequestHandler<GetFlowTasksByTaskIdQuery, IEnumerable<FlowTaskDto>>
{
    private readonly IFlowTaskRepository _flowTaskRepository;

    public GetFlowTasksByTaskIdQueryHandler(IFlowTaskRepository flowTaskRepository)
    {
        _flowTaskRepository = flowTaskRepository;
    }

    public async Task<IEnumerable<FlowTaskDto>> Handle(GetFlowTasksByTaskIdQuery request, CancellationToken cancellationToken)
    {
        var flowTasks = await _flowTaskRepository.GetByTaskIdAsync(request.TaskId, cancellationToken);
        
        return flowTasks.Select(flowTask => new FlowTaskDto
        {
            Id = flowTask.Id,
            WorkspaceId = flowTask.WorkspaceId,
            ProjectId = flowTask.ProjectId,
            ModuleId = flowTask.ModuleId,
            TaskId = flowTask.TaskId,
            Name = flowTask.Name,
            Description = flowTask.Description,
            SortOrder = flowTask.SortOrder,
            FlowStatus = (int)flowTask.FlowStatus
        }).ToList();
    }
}