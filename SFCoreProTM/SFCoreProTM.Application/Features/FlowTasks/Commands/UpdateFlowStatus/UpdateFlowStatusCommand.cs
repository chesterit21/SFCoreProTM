using System;
using MediatR;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Application.Features.FlowTasks.Commands.UpdateFlowStatus;

public class UpdateFlowStatusCommand : IRequest<Unit>
{
    public Guid FlowTaskId { get; set; }
    public FlowStatus Status { get; set; }
}