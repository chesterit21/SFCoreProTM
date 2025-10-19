using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;
using TaskEntity = SFCoreProTM.Domain.Entities.Projects.Task;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.UpdateSprintStatus;

public class UpdateSprintStatusCommandHandler : IRequestHandler<UpdateSprintStatusCommand, Unit>
{
    private readonly ISprintPlanningRepository _sprintPlanningRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSprintStatusCommandHandler(
        ISprintPlanningRepository sprintPlanningRepository,
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _sprintPlanningRepository = sprintPlanningRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateSprintStatusCommand request, CancellationToken cancellationToken)
    {
        var sprintPlanning = await _sprintPlanningRepository.GetByIdAsync(request.SprintPlanningId, cancellationToken);
        
        if (sprintPlanning == null)
        {
            throw new Exception($"SprintPlanning with ID {request.SprintPlanningId} not found.");
        }

        // Update sprint status
        sprintPlanning.UpdateStatus(request.Status);
        await _sprintPlanningRepository.UpdateAsync(sprintPlanning, cancellationToken);

        // Update related task statuses based on sprint status
        var task = await _taskRepository.GetByIdAsync(sprintPlanning.TaskId, cancellationToken);
        if (task != null)
        {
            switch (request.Status)
            {
                case SprintStatus.Pause:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.Paused);
                    break;
                case SprintStatus.Done:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.Completed);
                    break;
                case SprintStatus.Canceled:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.Paused);
                    break;
                case SprintStatus.InProgress:
                    task.UpdateStatus(Domain.Entities.Projects.TaskStatus.InProgress);
                    break;
            }
            
            await _taskRepository.UpdateAsync(task, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}