using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.DTOs.SprintPlannings;
using SFCoreProTM.Application.Features.SprintPlannings.Commands.CreateSprintPlanning;
using SFCoreProTM.Application.Features.SprintPlannings.Commands.DeleteSprintPlanning;
using SFCoreProTM.Application.Features.SprintPlannings.Commands.UpdateSprintPlanning;
using SFCoreProTM.Application.Features.SprintPlannings.Commands.UpdateSprintStatus;
using SFCoreProTM.Application.Features.SprintPlannings.Queries.GetSprintPlanningsByModuleId;
using SFCoreProTM.Domain.Entities.Projects;

namespace SFCoreProTM.Presentation.Controllers.Api;

[Route("api/modules/{moduleId:guid}/sprint-plannings")]
[ApiController]
public class SprintPlanningsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SprintPlanningsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetSprintPlanningsByModuleId(Guid moduleId)
    {
        var query = new GetSprintPlanningsByModuleIdQuery { ModuleId = moduleId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSprintPlanning(Guid moduleId, [FromBody] CreateSprintPlanningRequestDto request)
    {
        var command = new CreateSprintPlanningCommand { Request = request };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetSprintPlanningsByModuleId), new { moduleId = result.ModuleId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSprintPlanning(Guid moduleId, Guid id, [FromBody] UpdateSprintPlanningRequestDto request)
    {
        var command = new UpdateSprintPlanningCommand { SprintPlanningId = id, Request = request };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSprintPlanning(Guid moduleId, Guid id)
    {
        var command = new DeleteSprintPlanningCommand { SprintPlanningId = id };
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateSprintStatus(Guid moduleId, Guid id, [FromBody] int status)
    {
        if (!Enum.IsDefined(typeof(SprintStatus), status))
        {
            return BadRequest("Invalid sprint status");
        }

        var command = new UpdateSprintStatusCommand 
        { 
            SprintPlanningId = id, 
            Status = (SprintStatus)status 
        };
        
        await _mediator.Send(command);
        return NoContent();
    }
}