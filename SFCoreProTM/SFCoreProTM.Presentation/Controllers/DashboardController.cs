using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Presentation.Models;

namespace SFCoreProTM.Presentation.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult TaskBoard()
    {
        return View();
    }

    public IActionResult SprintPlan()
    {
        return View();
    }

    public IActionResult Backlog()
    {
        return View();
    }

    public IActionResult BurnDown()
    {
        return View();
    }

    public IActionResult TeamCapacity()
    {
        return View();
    }

    public IActionResult Release()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
