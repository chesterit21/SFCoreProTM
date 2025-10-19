using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SFCoreProTM.Presentation.Models;

namespace SFCoreProTM.Presentation.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {        // Jika pengguna sudah login, redirect ke dashboard
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Login()
    {
        // Jika pengguna sudah login, redirect ke dashboard
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Dashboard");
        }
        
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}