using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.DeleteFlowTask;

public class DeleteFlowTaskCommandHandler : IRequestHandler<DeleteFlowTaskCommand, Unit>
{
    private readonly IFlowTaskRepository _flowTaskRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteFlowTaskCommandHandler(IFlowTaskRepository flowTaskRepository, IUnitOfWork unitOfWork)
    {
        _flowTaskRepository = flowTaskRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteFlowTaskCommand request, CancellationToken cancellationToken)
    {
        var flowTask = await _flowTaskRepository.GetByIdAsync(request.FlowTaskId, cancellationToken);
        
        if (flowTask == null)
        {
            throw new Exception($"FlowTask with ID {request.FlowTaskId} not found.");
        }

        await _flowTaskRepository.DeleteAsync(flowTask, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}