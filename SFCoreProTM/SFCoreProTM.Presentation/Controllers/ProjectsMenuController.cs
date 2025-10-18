using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class ProjectsMenuController : Controller
{
   private readonly IMediator _mediator; // Inject jika perlu

    public ProjectsMenuController(IMediator mediator) // Inject jika perlu
    {
        _mediator = mediator;
    }

    // Action Index untuk menampilkan daftar proyek
    [HttpGet]
    [Route("workspaces/{workspaceId:guid}/projects")] // Tentukan route yang jelas
    public async Task<IActionResult> Index(Guid workspaceId)
    {
        // Kirim workspaceId ke View
        ViewData["WorkspaceId"] = workspaceId;

        // TODO: Fetch daftar proyek untuk workspaceId ini
        // var projectsQuery = new GetProjectsByWorkspaceQuery(workspaceId);
        // var projects = await _mediator.Send(projectsQuery);
        // return View(projects); // Kirim daftar proyek sebagai Model

        // Untuk sekarang, kita hanya tampilkan view kosong
        return View(); // Akan mencari Views/ProjectsMenu/Index.cshtml
    }
    // Action BARU untuk menampilkan form Create Project
    [HttpGet]
    // Tentukan route yang sesuai, misal berdasarkan workspace
    [Route("workspaces/{workspaceId:guid}/projects/new-ui")]
    public IActionResult Create(Guid workspaceId)
    {
        // Kirim workspaceId ke view agar bisa digunakan oleh JavaScript
        ViewData["WorkspaceId"] = workspaceId;

        // Opsional: Ambil data user/members untuk dropdown Project Lead/Assignee
        // ViewData["Users"] = GetUsersForDropdown(workspaceId);

        // Gunakan DTO atau Request object sebagai model jika perlu pre-fill
        var model = new SFCoreProTM.Application.Mapping.Requests.Projects.CreateProjectRequest();
        return View(model); // Akan mencari Views/ProjectsMenu/Create.cshtml
    }

    // Helper untuk mendapatkan User ID (pindahkan ke base controller atau service jika sering dipakai)
    private Guid GetUserId()
    {
        var sub = User?.FindFirst("sub")?.Value;
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }
}
