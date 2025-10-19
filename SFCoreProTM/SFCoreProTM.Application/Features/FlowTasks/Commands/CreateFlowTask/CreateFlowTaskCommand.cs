using MediatR;
using SFCoreProTM.Application.DTOs.FlowTasks;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.CreateFlowTask;

public class CreateFlowTaskCommand : IRequest<FlowTaskDto>
{
    public CreateFlowTaskRequestDto Request { get; set; } = new();
}