using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class ResourcesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
