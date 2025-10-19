using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.UpdateFlowStatus;

public class UpdateFlowStatusCommandHandler : IRequestHandler<UpdateFlowStatusCommand, Unit>
{
    private readonly IFlowTaskRepository _flowTaskRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateFlowStatusCommandHandler(
        IFlowTaskRepository flowTaskRepository,
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _flowTaskRepository = flowTaskRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateFlowStatusCommand request, CancellationToken cancellationToken)
    {
        var flowTask = await _flowTaskRepository.GetByIdAsync(request.FlowTaskId, cancellationToken);
        
        if (flowTask == null)
        {
            throw new Exception($"FlowTask with ID {request.FlowTaskId} not found.");
        }

        // Update flow task status
        flowTask.UpdateStatus(request.Status);
        await _flowTaskRepository.UpdateAsync(flowTask, cancellationToken);

        // Update related task statuses based on flow status
        var task = await _taskRepository.GetByIdAsync(flowTask.TaskId, cancellationToken);
        if (task != null)
        {
            switch (request.Status)
            {
                case FlowStatus.Pause:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.Paused);
                    break;
                case FlowStatus.Done:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.Completed);
                    break;
                case FlowStatus.Canceled:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.Paused);
                    break;
                case FlowStatus.InProgress:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.InProgress);
                    break;
            }
            
            await _taskRepository.UpdateAsync(task, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}