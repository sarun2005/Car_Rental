using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
