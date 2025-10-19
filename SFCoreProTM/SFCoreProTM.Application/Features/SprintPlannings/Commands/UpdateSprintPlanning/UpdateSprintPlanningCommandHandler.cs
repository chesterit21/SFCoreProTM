using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.SprintPlannings;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.UpdateSprintPlanning;

public class UpdateSprintPlanningCommandHandler : IRequestHandler<UpdateSprintPlanningCommand, SprintPlanningDto>
{
    private readonly ISprintPlanningRepository _sprintPlanningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSprintPlanningCommandHandler(ISprintPlanningRepository sprintPlanningRepository, IUnitOfWork unitOfWork)
    {
        _sprintPlanningRepository = sprintPlanningRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SprintPlanningDto> Handle(UpdateSprintPlanningCommand request, CancellationToken cancellationToken)
    {
        var sprintPlanning = await _sprintPlanningRepository.GetByIdAsync(request.SprintPlanningId, cancellationToken);
        
        if (sprintPlanning == null)
        {
            throw new Exception($"SprintPlanning with ID {request.SprintPlanningId} not found.");
        }

        sprintPlanning.UpdateDetails(
            request.Request.Name,
            request.Request.Description,
            request.Request.StartDate,
            request.Request.TargetDate,
            request.Request.SortOrder,
            sprintPlanning.SprintStatus,
            request.Request.Note
        );

        await _sprintPlanningRepository.UpdateAsync(sprintPlanning, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new SprintPlanningDto
        {
            Id = sprintPlanning.Id,
            WorkspaceId = sprintPlanning.WorkspaceId,
            ProjectId = sprintPlanning.ProjectId,
            ModuleId = sprintPlanning.ModuleId,
            TaskId = sprintPlanning.TaskId,
            Name = sprintPlanning.Name,
            Description = sprintPlanning.Description,
            StartDate = sprintPlanning.StartDate,
            TargetDate = sprintPlanning.TargetDate,
            SortOrder = sprintPlanning.SortOrder,
            SprintStatus = (int)sprintPlanning.SprintStatus,
            Note = sprintPlanning.Note
        };
    }
}