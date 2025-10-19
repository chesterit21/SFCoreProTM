using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.FlowTasks;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.UpdateFlowTask;

public class UpdateFlowTaskCommandHandler : IRequestHandler<UpdateFlowTaskCommand, FlowTaskDto>
{
    private readonly IFlowTaskRepository _flowTaskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFlowTaskCommandHandler(IFlowTaskRepository flowTaskRepository, IUnitOfWork unitOfWork)
    {
        _flowTaskRepository = flowTaskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<FlowTaskDto> Handle(UpdateFlowTaskCommand request, CancellationToken cancellationToken)
    {
        var flowTask = await _flowTaskRepository.GetByIdAsync(request.FlowTaskId, cancellationToken);
        
        if (flowTask == null)
        {
            throw new Exception($"FlowTask with ID {request.FlowTaskId} not found.");
        }

        flowTask.UpdateDetails(
            request.Request.Name,
            request.Request.Description,
            request.Request.SortOrder,
            flowTask.FlowStatus
        );

        await _flowTaskRepository.UpdateAsync(flowTask, cancellationToken);
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