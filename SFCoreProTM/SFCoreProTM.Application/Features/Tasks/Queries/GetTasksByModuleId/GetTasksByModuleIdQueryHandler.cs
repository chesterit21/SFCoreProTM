using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Tasks;
using SFCoreProTM.Application.Interfaces.Repositories;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Application.Features.Tasks.Queries.GetTasksByModuleId;

public class GetTasksByModuleIdQueryHandler : IRequestHandler<GetTasksByModuleIdQuery, IEnumerable<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTasksByModuleIdQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskDto>> Handle(GetTasksByModuleIdQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByModuleIdAsync(request.ModuleId, cancellationToken);

        return tasks.Select(task => new TaskDto
        {
            Id = task.Id,
            WorkspaceId = task.WorkspaceId,
            ProjectId = task.ProjectId,
            ModuleId = task.ModuleId,
            Name = task.Name,
            Description = task.Description,
            SortOrder = task.SortOrder,
            Status = (int)task.Status,
            IsErd = task.IsErd
        }).OrderBy(task => task.SortOrder).ToList();
    }
}