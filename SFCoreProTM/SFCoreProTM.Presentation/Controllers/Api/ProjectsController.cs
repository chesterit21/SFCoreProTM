using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SFCoreProTM.Application.DTOs.Projects;
using SFCoreProTM.Application.Features.Projects.Commands.CreateProject;
using SFCoreProTM.Application.Mapping.Requests.Projects;

namespace SFCoreProTM.Presentation.Controllers;

[ApiController]
[Authorize]
[Route("api/workspaces/{workspaceId:guid}/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProjectsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(Guid workspaceId, [FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var dto = _mapper.Map<CreateProjectRequestDto>(request);

        var command = new CreateProjectCommand(workspaceId, request.ActorId, dto);
        var project = await _mediator.Send(command, cancellationToken);

        return Created($"/api/workspaces/{workspaceId}/projects/{project.Id}", project);
    }
}
