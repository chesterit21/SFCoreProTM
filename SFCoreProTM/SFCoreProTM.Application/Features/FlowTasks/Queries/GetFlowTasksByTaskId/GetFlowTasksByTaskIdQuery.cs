using System;
using System.Collections.Generic;
using MediatR;
using SFCoreProTM.Application.DTOs.FlowTasks;

namespace SFCoreProTM.Application.Features.FlowTasks.Queries.GetFlowTasksByTaskId;

public class GetFlowTasksByTaskIdQuery : IRequest<IEnumerable<FlowTaskDto>>
{
    public Guid TaskId { get; set; }
}