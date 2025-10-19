using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFCoreProTM.Application.DTOs.SprintPlannings;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Application.Interfaces.Repositories;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.CreateSprintPlanning;

public class CreateSprintPlanningCommandHandler : IRequestHandler<CreateSprintPlanningCommand, SprintPlanningDto>
{
    private readonly ISprintPlanningRepository _sprintPlanningRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSprintPlanningCommandHandler(ISprintPlanningRepository sprintPlanningRepository, IUnitOfWork unitOfWork)
    {
        _sprintPlanningRepository = sprintPlanningRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SprintPlanningDto> Handle(CreateSprintPlanningCommand request, CancellationToken cancellationToken)
    {
        var sprintPlanning = SprintPlanning.Create(
            Guid.NewGuid(),
            request.Request.WorkspaceId,
            request.Request.ProjectId,
            request.Request.ModuleId,
            request.Request.TaskId,
            request.Request.Name,
            request.Request.Description,
            request.Request.StartDate,
            request.Request.TargetDate,
            request.Request.SortOrder,
            SprintStatus.InProgress, // Default status
            request.Request.Note
        );

        await _sprintPlanningRepository.AddAsync(sprintPlanning, cancellationToken);
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