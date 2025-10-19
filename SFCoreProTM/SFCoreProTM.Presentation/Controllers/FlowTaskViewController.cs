using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class FlowTaskViewController : Controller
{
    public IActionResult Index(Guid taskId)
    {
        ViewBag.TaskId = taskId;
        return View();
    }

    public IActionResult CreateModal(Guid taskId)
    {
        ViewBag.TaskId = taskId;
        return PartialView("_CreateModal");
    }

    public IActionResult EditModal(Guid taskId, Guid flowTaskId)
    {
        ViewBag.TaskId = taskId;
        ViewBag.FlowTaskId = flowTaskId;
        return PartialView("_EditModal");
    }
}