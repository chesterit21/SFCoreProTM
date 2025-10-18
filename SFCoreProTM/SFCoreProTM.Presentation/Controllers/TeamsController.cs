using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class TeamsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
