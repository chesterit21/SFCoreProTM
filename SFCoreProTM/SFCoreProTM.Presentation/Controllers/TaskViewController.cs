using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class TaskViewController : Controller
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

    public IActionResult EditModal(Guid moduleId, Guid taskId)
    {
        ViewBag.ModuleId = moduleId;
        ViewBag.TaskId = taskId;
        return PartialView("_EditModal");
    }
}