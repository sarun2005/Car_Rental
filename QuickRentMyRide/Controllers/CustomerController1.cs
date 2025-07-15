using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Data;

namespace QuickRentMyRide.Controllers
{
    public class CustomerController1 : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerController1(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
