using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.DTOs.FlowTasks;
using SFCoreProTM.Application.Features.FlowTasks.Commands.CreateFlowTask;
using SFCoreProTM.Application.Features.FlowTasks.Commands.DeleteFlowTask;
using SFCoreProTM.Application.Features.FlowTasks.Commands.UpdateFlowStatus;
using SFCoreProTM.Application.Features.FlowTasks.Commands.UpdateFlowTask;
using SFCoreProTM.Application.Features.FlowTasks.Queries.GetFlowTasksByTaskId;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Presentation.Controllers.Api;

[Route("api/tasks/{taskId:guid}/flow-tasks")]
[ApiController]
public class FlowTasksController : ControllerBase
{
    private readonly IMediator _mediator;

    public FlowTasksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetFlowTasksByTaskId(Guid taskId)
    {
        var query = new GetFlowTasksByTaskIdQuery { TaskId = taskId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlowTask(Guid taskId, [FromBody] CreateFlowTaskRequestDto request)
    {
        var command = new CreateFlowTaskCommand { Request = request };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetFlowTasksByTaskId), new { taskId = result.TaskId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFlowTask(Guid taskId, Guid id, [FromBody] UpdateFlowTaskRequestDto request)
    {
        var command = new UpdateFlowTaskCommand { FlowTaskId = id, Request = request };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFlowTask(Guid taskId, Guid id)
    {
        var command = new DeleteFlowTaskCommand { FlowTaskId = id };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateFlowStatus(Guid taskId, Guid id, [FromBody] int status)
    {
        if (!Enum.IsDefined(typeof(FlowStatus), status))
        {
            return BadRequest("Invalid flow status");
        }

        var command = new UpdateFlowStatusCommand 
        { 
            FlowTaskId = id, 
            Status = (FlowStatus)status 
        };
        
        await _mediator.Send(command);
        return NoContent();
    }
}