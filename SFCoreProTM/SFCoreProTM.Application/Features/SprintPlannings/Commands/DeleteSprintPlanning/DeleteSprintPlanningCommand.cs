using System;
using MediatR;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.DeleteSprintPlanning;

public class DeleteSprintPlanningCommand : IRequest<Unit>
{
    public Guid SprintPlanningId { get; set; }
}