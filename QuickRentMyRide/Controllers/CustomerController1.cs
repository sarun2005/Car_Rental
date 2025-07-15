using Microsoft.AspNetCore.Mvc;

namespace QuickRentMyRide.Controllers
{
    public class CustomerController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
