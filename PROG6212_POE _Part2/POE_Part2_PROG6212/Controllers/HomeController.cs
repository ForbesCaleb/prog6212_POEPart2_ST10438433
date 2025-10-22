using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POE_Part2_PROG6212.Models;

namespace POE_Part_2_PROG6212.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Public welcome/landing page. If the user is already authenticated,
        /// we skip this page and send them to the dashboard.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        /// <summary>
        /// Optional public about page describing the system.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Keep your existing Privacy page public.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

