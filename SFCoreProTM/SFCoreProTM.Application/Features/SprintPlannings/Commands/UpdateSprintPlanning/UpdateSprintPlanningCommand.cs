using System;
using MediatR;
using SFCoreProTM.Application.DTOs.SprintPlannings;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.UpdateSprintPlanning;

public class UpdateSprintPlanningCommand : IRequest<SprintPlanningDto>
{
    public Guid SprintPlanningId { get; set; }
    public UpdateSprintPlanningRequestDto Request { get; set; } = new();
}