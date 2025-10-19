using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.DTOs.ErdDefinitions;
using SFCoreProTM.Application.Features.ErdDefinitions.Commands.CreateErdDefinition;
using SFCoreProTM.Application.Features.ErdDefinitions.Commands.DeleteErdDefinition;
using SFCoreProTM.Application.Features.ErdDefinitions.Commands.UpdateErdDefinition;
using SFCoreProTM.Application.Features.ErdDefinitions.Queries.GetErdDefinitionsByModuleId;

namespace SFCoreProTM.Presentation.Controllers.Api;

[Route("api/modules/{moduleId:guid}/erd-definitions")]
[ApiController]
public class ErdDefinitionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ErdDefinitionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetErdDefinitionsByModuleId(Guid moduleId)
    {
        var query = new GetErdDefinitionsByModuleIdQuery { ModuleId = moduleId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateErdDefinition(Guid moduleId, [FromBody] CreateErdDefinitionRequestDto request)
    {
        var command = new CreateErdDefinitionCommand { Request = request };
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetErdDefinitionsByModuleId), new { moduleId = result.ModuleId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateErdDefinition(Guid moduleId, Guid id, [FromBody] UpdateErdDefinitionRequestDto request)
    {
        var command = new UpdateErdDefinitionCommand { ErdDefinitionId = id, Request = request };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteErdDefinition(Guid moduleId, Guid id)
    {
        var command = new DeleteErdDefinitionCommand { ErdDefinitionId = id };
        await _mediator.Send(command);
        return NoContent();
    }
}