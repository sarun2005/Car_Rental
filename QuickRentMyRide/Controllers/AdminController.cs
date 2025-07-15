using Microsoft.AspNetCore.Mvc;

namespace QuickRentMyRide.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
