using System;
using MediatR;

namespace SFCoreProTM.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand : IRequest<bool>
{
    public Guid ActorId { get; set; }
    public Guid TaskId { get; set; }
}