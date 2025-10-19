using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.DTOs.Tasks;
using SFCoreProTM.Application.Features.Tasks.Commands.CreateTask;
using SFCoreProTM.Application.Features.Tasks.Commands.DeleteTask;
using SFCoreProTM.Application.Features.Tasks.Commands.UpdateTask;
using SFCoreProTM.Application.Features.Tasks.Queries.GetTasksByModuleId;
using SFCoreProTM.Application.Interfaces.Security;

namespace SFCoreProTM.Presentation.Controllers.Api;

[ApiController]
[Route("api/modules/{moduleId:guid}/tasks")]
public class TasksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public TasksController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksByModuleId(Guid moduleId)
    {
        var query = new GetTasksByModuleIdQuery { ModuleId = moduleId };
        var tasks = await _mediator.Send(query);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(Guid moduleId, [FromBody] CreateTaskRequestDto request)
    {
        var command = new CreateTaskCommand
        {
            ActorId = _currentUserService.UserId ?? Guid.Empty,
            Request = request
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(Guid moduleId, Guid taskId, [FromBody] UpdateTaskRequestDto request)
    {
        var command = new UpdateTaskCommand
        {
            ActorId = _currentUserService.UserId ?? Guid.Empty,
            TaskId = taskId,
            Request = request
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid moduleId, Guid taskId)
    {
        var command = new DeleteTaskCommand
        {
            ActorId = _currentUserService.UserId ?? Guid.Empty,
            TaskId = taskId
        };

        var result = await _mediator.Send(command);
        if (result)
        {
            return Ok(new { Message = "Task deleted successfully" });
        }
        else
        {
            return NotFound(new { Message = "Task not found" });
        }
    }
}