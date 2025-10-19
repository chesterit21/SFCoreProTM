using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.Tasks;

namespace SFCoreProTM.Application.Features.Tasks.Queries.GetTasksByModuleId;

public class GetTasksByModuleIdQuery : IRequest<IEnumerable<TaskDto>>
{
    public Guid ModuleId { get; set; }
}