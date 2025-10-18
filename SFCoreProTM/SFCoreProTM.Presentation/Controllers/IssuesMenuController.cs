using Microsoft.AspNetCore.Mvc;

namespace SFCoreProTM.Presentation.Controllers;

public class IssuesMenuController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
