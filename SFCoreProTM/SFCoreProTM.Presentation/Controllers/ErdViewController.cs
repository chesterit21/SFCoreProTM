using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class ErdViewController : Controller
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

    public IActionResult EditModal(Guid moduleId, Guid erdDefinitionId)
    {
        ViewBag.ModuleId = moduleId;
        ViewBag.ErdDefinitionId = erdDefinitionId;
        return PartialView("_EditModal");
    }
}