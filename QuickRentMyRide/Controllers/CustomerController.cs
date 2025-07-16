using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Data;

namespace QuickRentMyRide.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerController(ApplicationDbContext dbContext)
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
