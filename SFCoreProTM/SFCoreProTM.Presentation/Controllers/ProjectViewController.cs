using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SFCoreProTM.Application.Features.Projects.Queries.GetProjectsByWorkspace;

namespace SFCoreProTM.Presentation.Controllers;

[Authorize]
public class ProjectViewController : Controller
{
    private readonly IMediator _mediator;

    public ProjectViewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("workspaces/{workspaceId:guid}/projects")]
    public async Task<IActionResult> Index(Guid workspaceId)
    {
        var query = new GetProjectsByWorkspaceQuery(workspaceId);
        var projects = await _mediator.Send(query);
        ViewData["WorkspaceId"] = workspaceId;
        return View(projects);
    }
    
    [HttpGet("workspaces/{workspaceId:guid}/projects/{projectId:guid}/edit")]
    public IActionResult Edit(Guid workspaceId, Guid projectId)
    {
        ViewData["WorkspaceId"] = workspaceId;
        ViewData["ProjectId"] = projectId;
        return View();
    }
}