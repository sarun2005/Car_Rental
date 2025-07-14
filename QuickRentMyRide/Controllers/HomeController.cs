using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Models;

namespace QuickRentMyRide.Controllers
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
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (ModelState.IsValid)
            {
               
                if (user.Email == "admin@example.com" && user.Password == "admin123")
                {
                    
                    return RedirectToAction("Index");
                }

                
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
            }

            return View(user);
        }


    }
}
