using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Tasks;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskCommandHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        
        if (task == null)
        {
            throw new Exception($"Task with ID {request.TaskId} not found.");
        }

        task.UpdateDetails(
            request.Request.Name,
            request.Request.Description,
            request.Request.SortOrder
        );

        await _taskRepository.UpdateAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TaskDto
        {
            Id = task.Id,
            WorkspaceId = task.WorkspaceId,
            ProjectId = task.ProjectId,
            ModuleId = task.ModuleId,
            Name = task.Name,
            Description = task.Description,
            SortOrder = task.SortOrder,
            Status = (int)task.Status
        };
    }
}