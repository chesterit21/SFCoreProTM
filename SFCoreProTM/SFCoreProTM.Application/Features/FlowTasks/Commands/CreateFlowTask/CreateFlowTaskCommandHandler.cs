using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.FlowTasks;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.CreateFlowTask;

public class CreateFlowTaskCommandHandler : IRequestHandler<CreateFlowTaskCommand, FlowTaskDto>
{
    private readonly IFlowTaskRepository _flowTaskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateFlowTaskCommandHandler(IFlowTaskRepository flowTaskRepository, IUnitOfWork unitOfWork)
    {
        _flowTaskRepository = flowTaskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlowTaskDto> Handle(CreateFlowTaskCommand request, CancellationToken cancellationToken)
    {
        var flowTask = FlowTask.Create(
            Guid.NewGuid(),
            request.Request.WorkspaceId,
            request.Request.ProjectId,
            request.Request.ModuleId,
            request.Request.TaskId,
            request.Request.Name,
            request.Request.Description,
            request.Request.SortOrder,
            FlowStatus.InProgress // Default status
        );

        await _flowTaskRepository.AddAsync(flowTask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new FlowTaskDto
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
        };
    }
}