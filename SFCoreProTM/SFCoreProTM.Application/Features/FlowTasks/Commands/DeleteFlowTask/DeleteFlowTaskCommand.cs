using System;
using MediatR;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.DeleteFlowTask;

public class DeleteFlowTaskCommand : IRequest<Unit>
{
    public Guid FlowTaskId { get; set; }
}