using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class SprintViewController : Controller
{
    public IActionResult Index(Guid moduleId)
    {
        ViewBag.ModuleId = moduleId;
        return View();
    }

    public IActionResult CreateModal(Guid moduleId)
    {
        ViewBag.ModuleId = moduleId;
        return PartialView("_CreateModal");
    }

    public IActionResult EditModal(Guid moduleId, Guid sprintPlanningId)
    {
        ViewBag.ModuleId = moduleId;
        ViewBag.SprintPlanningId = sprintPlanningId;
        return PartialView("_EditModal");
    }
}