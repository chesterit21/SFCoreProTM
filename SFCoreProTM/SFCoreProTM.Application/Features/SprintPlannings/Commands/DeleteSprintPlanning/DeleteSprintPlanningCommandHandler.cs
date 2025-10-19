using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.DeleteSprintPlanning;

public class DeleteSprintPlanningCommandHandler : IRequestHandler<DeleteSprintPlanningCommand, Unit>
{
    private readonly ISprintPlanningRepository _sprintPlanningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSprintPlanningCommandHandler(ISprintPlanningRepository sprintPlanningRepository, IUnitOfWork unitOfWork)
    {
        _sprintPlanningRepository = sprintPlanningRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteSprintPlanningCommand request, CancellationToken cancellationToken)
    {
        var sprintPlanning = await _sprintPlanningRepository.GetByIdAsync(request.SprintPlanningId, cancellationToken);
        
        if (sprintPlanning == null)
        {
            throw new Exception($"SprintPlanning with ID {request.SprintPlanningId} not found.");
        }

        await _sprintPlanningRepository.DeleteAsync(sprintPlanning, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}