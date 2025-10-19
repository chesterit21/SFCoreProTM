using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Tasks;

namespace SFCoreProTM.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest<TaskDto>
{
    public Guid ActorId { get; set; }
    public Guid TaskId { get; set; }
    public UpdateTaskRequestDto Request { get; set; } = new();
}