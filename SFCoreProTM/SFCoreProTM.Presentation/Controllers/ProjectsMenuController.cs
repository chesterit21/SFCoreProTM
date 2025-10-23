using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Application.Interfaces.Security;
using SFCoreProTM.Application.Features.Projects.Queries.GetProjectById;

namespace SFCoreProTM.Presentation.Controllers;

public class ProjectsMenuController : Controller
{
   private readonly IMediator _mediator; // Inject jika perlu
   private readonly ICurrentUserService _currentUserService;

    public ProjectsMenuController(IMediator mediator, ICurrentUserService currentUserService) // Inject jika perlu
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task<IActionResult> Index()
    {
        var workspaceId = _currentUserService.WorkspaceId;
        if (workspaceId is null)
        {
            // Or redirect to a workspace selection page
            return View(new List<SFCoreProTM.Application.DTOs.Projects.ProjectSummaryDto>());
        }

        ViewData["WorkspaceId"] = workspaceId.Value;

        try
        {
            var projectsQuery = new SFCoreProTM.Application.Features.Projects.Queries.GetProjectsByWorkspace.GetProjectsByWorkspaceQuery(workspaceId.Value);
            var projects = await _mediator.Send(projectsQuery);
            
            // Verifikasi hasil query
            if (projects.Count() == 0)
            {
                projects = new List<SFCoreProTM.Application.DTOs.Projects.ProjectSummaryDto>();
            }
            
            return View(projects);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // Catat informasi pengecualian
            // _logger.LogError(ex, "Terjadi kesalahan saat mengambil daftar proyek");
            ViewData["ErrorMessage"] = "Terjadi kesalahan saat memuat daftar proyek, silakan coba lagi nanti.";
            return View(new List<SFCoreProTM.Application.DTOs.Projects.ProjectSummaryDto>());
        }
    }
    
    // Action BARU untuk menampilkan form Create Project
    [HttpGet]
    public IActionResult Create()
    {
        // Kirim workspaceId ke view agar bisa digunakan oleh JavaScript
        ViewData["WorkspaceId"] = _currentUserService.WorkspaceId;

        // Opsional: Ambil data user/members untuk dropdown Project Lead/Assignee
        // ViewData["Users"] = GetUsersForDropdown(workspaceId);

        // Gunakan DTO atau Request object sebagai model jika perlu pre-fill
        var model = new SFCoreProTM.Application.Mapping.Requests.Projects.CreateProjectRequest();
        return View(model); // Akan mencari Views/ProjectsMenu/Create.cshtml
    }
    
    // Action untuk menampilkan form Edit Project
    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        // Kirim workspaceId dan projectId ke view agar bisa digunakan oleh JavaScript
        ViewData["WorkspaceId"] = _currentUserService.WorkspaceId;
        ViewData["ProjectId"] = id;

        return View(); // Akan mencari Views/ProjectsMenu/Edit.cshtml
    }
    
    // Action untuk redirect ke ModuleView
    [HttpGet]
    public IActionResult Modules(Guid id)
    {
        // id adalah projectId
        return RedirectToAction("Index", "ModuleView", new { projectId = id });
    }
}