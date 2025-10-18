using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class WorkspacesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

}
