using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SFCoreProTM.Application.Features.Projects.Queries.GetProjectById;
using SFCoreProTM.Application.Interfaces.Security;

namespace SFCoreProTM.Presentation.Controllers;

[Authorize]
public class ModuleViewController : Controller
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ModuleViewController(IMediator mediator, ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(Guid projectId)
    {
        ViewBag.ProjectId = projectId;
        ViewBag.WorkspaceId = _currentUserService.WorkspaceId;
        
        try 
        {
            var projectQuery = new GetProjectByIdQuery(projectId);
            var project = await _mediator.Send(projectQuery);
            ViewBag.ProjectName = project?.Name ?? "Project";
        }
        catch (Exception)
        {
            ViewBag.ProjectName = "Project";
        }
        
        return View();
    }

    public IActionResult CreateModal(Guid projectId)
    {
        ViewBag.ProjectId = projectId;
        ViewBag.WorkspaceId = _currentUserService.WorkspaceId;
        return PartialView("_CreateModal");
    }

    public IActionResult EditModal(Guid projectId, Guid moduleId)
    {
        ViewBag.ProjectId = projectId;
        ViewBag.ModuleId = moduleId;
        return PartialView("_EditModal");
    }
}