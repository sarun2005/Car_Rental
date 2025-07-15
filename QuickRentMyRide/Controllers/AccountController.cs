using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Data;
using QuickRentMyRide.Models;

namespace QuickRentMyRide.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AccountController (ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        // Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Admin Hardcoded Login Check
            if (user.Email == "Admin@gmail.com" &&  user.Password =="Admin@123")
            {
                TempData["Login Success"] = "Welcom Admin!";
                return RedirectToAction("Dashboard", "Admin");
            }


            // Customer Login (From DB)
            var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                TempData["LoginSuccess"] = "Welcome Customer!";
                return RedirectToAction("Dashboard", "Customer");
            }

            ModelState.AddModelError("", "Invalid Email or Password.");
            return View(user);
        }

        // Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            { 
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                TempData["RegisterSuccess"] = "Account created successfully! Please login.";
                return RedirectToAction("Login");
            }
            return View(user);
        }  
    }
}
