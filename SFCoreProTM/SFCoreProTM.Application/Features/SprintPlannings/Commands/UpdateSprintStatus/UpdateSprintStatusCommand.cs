using System;
using MediatR;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.SprintPlannings.Commands.UpdateSprintStatus;

public class UpdateSprintStatusCommand : IRequest<Unit>
{
    public Guid SprintPlanningId { get; set; }
    public SprintStatus Status { get; set; }
}