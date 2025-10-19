using MediatR;
using SFCoreProTM.Application.DTOs.SprintPlannings;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.CreateSprintPlanning;

public class CreateSprintPlanningCommand : IRequest<SprintPlanningDto>
{
    public CreateSprintPlanningRequestDto Request { get; set; } = new();
}