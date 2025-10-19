using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.Tasks;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var task = TaskEntity.Create(
            Guid.NewGuid(),
            request.Request.WorkspaceId,
            request.Request.ProjectId,
            request.Request.ModuleId,
            request.Request.Name,
            request.Request.Description,
            request.Request.SortOrder,
            request.Request.IsErd
        );

        await _taskRepository.AddAsync(task, cancellationToken);
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
            Status = (int)task.Status,
            IsErd = task.IsErd
        };
    }
}