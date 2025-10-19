using System;
using MediatR;
using SFCoreProTM.Application.DTOs.Tasks;

namespace SFCoreProTM.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommand : IRequest<TaskDto>
{
    public Guid ActorId { get; set; }
    public CreateTaskRequestDto Request { get; set; } = new();
}