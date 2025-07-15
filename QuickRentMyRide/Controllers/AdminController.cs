using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Data;

namespace QuickRentMyRide.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
