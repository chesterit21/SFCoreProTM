using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;

namespace SFCoreProTM.Application.Features.Modules.Commands.DeleteModule;

public class DeleteModuleCommandHandler : IRequestHandler<DeleteModuleCommand, bool>
{
    private readonly IModuleRepository _moduleRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IFlowTaskRepository _flowTaskRepository;
    private readonly IErdDefinitionRepository _erdDefinitionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteModuleCommandHandler(
        IModuleRepository moduleRepository, 
        ITaskRepository taskRepository,
        IFlowTaskRepository flowTaskRepository,
        IErdDefinitionRepository erdDefinitionRepository,
        IUnitOfWork unitOfWork)
    {
        _moduleRepository = moduleRepository;
        _taskRepository = taskRepository;
        _flowTaskRepository = flowTaskRepository;
        _erdDefinitionRepository = erdDefinitionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteModuleCommand request, CancellationToken cancellationToken)
    {
        var module = await _moduleRepository.GetByIdAsync(request.ModuleId, cancellationToken);
        
        if (module == null)
        {
            return false;
        }

        // Get all tasks for this module
        var tasks = await _taskRepository.GetByModuleIdAsync(request.ModuleId, cancellationToken);
        
        // Delete all flow tasks and ERD definitions for each task
        foreach (var task in tasks)
        {
            // Delete flow tasks for this task
            var flowTasks = await _flowTaskRepository.GetByTaskIdAsync(task.Id, cancellationToken);
            foreach (var flowTask in flowTasks)
            {
                await _flowTaskRepository.DeleteAsync(flowTask, cancellationToken);
            }
            
            // Delete the task itself
            await _taskRepository.DeleteAsync(task, cancellationToken);
        }
        
        // Delete all ERD definitions for this module
        var erdDefinitions = await _erdDefinitionRepository.GetByModuleIdAsync(request.ModuleId, cancellationToken);
        foreach (var erdDefinition in erdDefinitions)
        {
            await _erdDefinitionRepository.DeleteAsync(erdDefinition, cancellationToken);
        }

        await _moduleRepository.DeleteAsync(module, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}