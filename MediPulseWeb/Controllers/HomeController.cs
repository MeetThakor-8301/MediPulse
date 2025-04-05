using System.Diagnostics;
using MediPulseWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediPulseWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("SuperAdmin"))
                {
                    return RedirectToAction("Doctors", "Admin");
                }
                else if (User.IsInRole("Doctor"))
                {
                    return RedirectToAction("Appointments", "Doctor");
                }
                else if (User.IsInRole("Patient"))
                {
                    return RedirectToAction("Doctors", "Patient");
                }
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
