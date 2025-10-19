using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.SprintPlannings;

namespace SFCoreProTM.Application.Features.SprintPlannings.Queries.GetSprintPlanningsByModuleId;

public class GetSprintPlanningsByModuleIdQuery : IRequest<IEnumerable<SprintPlanningDto>>
{
    public Guid ModuleId { get; set; }
}