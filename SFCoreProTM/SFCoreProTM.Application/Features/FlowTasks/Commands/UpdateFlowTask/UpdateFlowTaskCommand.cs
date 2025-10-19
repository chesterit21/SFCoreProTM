using System;
using MediatR;
using SFCoreProTM.Application.DTOs.FlowTasks;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.UpdateFlowTask;

public class UpdateFlowTaskCommand : IRequest<FlowTaskDto>
{
    public Guid FlowTaskId { get; set; }
    public UpdateFlowTaskRequestDto Request { get; set; } = new();
}