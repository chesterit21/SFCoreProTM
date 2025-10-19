using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.DTOs.Modules;
using SFCoreProTM.Application.Features.Modules.Commands.CreateModule;
using SFCoreProTM.Application.Features.Modules.Commands.DeleteModule;
using SFCoreProTM.Application.Features.Modules.Commands.UpdateModule;
using SFCoreProTM.Application.Features.Modules.Queries.GetModulesByProjectId;
using SFCoreProTM.Application.Interfaces.Security;

namespace SFCoreProTM.Presentation.Controllers.Api;

[ApiController]
[Route("api/projects/{projectId:guid}/modules")]
public class ModulesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ModulesController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetModulesByProjectId(Guid projectId)
    {
        var query = new GetModulesByProjectIdQuery { ProjectId = projectId };
        var modules = await _mediator.Send(query);
        return Ok(modules);
    }

    [HttpPost]
    public async Task<IActionResult> CreateModule(Guid projectId, [FromBody] CreateModuleRequestDto request)
    {
        var command = new CreateModuleCommand
        {
            ActorId = _currentUserService.UserId ?? Guid.Empty,
            Request = request
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{moduleId:guid}")]
    public async Task<IActionResult> UpdateModule(Guid projectId, Guid moduleId, [FromBody] UpdateModuleRequestDto request)
    {
        var command = new UpdateModuleCommand
        {
            ActorId = _currentUserService.UserId ?? Guid.Empty,
            ModuleId = moduleId,
            Request = request
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{moduleId:guid}")]
    public async Task<IActionResult> DeleteModule(Guid projectId, Guid moduleId)
    {
        var command = new DeleteModuleCommand
        {
            ActorId = _currentUserService.UserId ?? Guid.Empty,
            ModuleId = moduleId
        };

        var result = await _mediator.Send(command);
        if (result)
        {
            return Ok(new { Message = "Module deleted successfully" });
        }
        else
        {
            return NotFound(new { Message = "Module not found" });
        }
    }
}