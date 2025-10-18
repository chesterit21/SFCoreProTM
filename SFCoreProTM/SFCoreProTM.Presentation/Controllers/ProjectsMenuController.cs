using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class ProjectsMenuController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
