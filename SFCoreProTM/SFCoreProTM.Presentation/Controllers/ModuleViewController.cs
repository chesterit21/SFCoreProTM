using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class ModuleViewController : Controller
{
    public IActionResult Index(Guid projectId)
    {
        ViewBag.ProjectId = projectId;
        return View();
    }

    public IActionResult CreateModal(Guid projectId)
    {
        ViewBag.ProjectId = projectId;
        return PartialView("_CreateModal");
    }

    public IActionResult EditModal(Guid projectId, Guid moduleId)
    {
        ViewBag.ProjectId = projectId;
        ViewBag.ModuleId = moduleId;
        return PartialView("_EditModal");
    }
}