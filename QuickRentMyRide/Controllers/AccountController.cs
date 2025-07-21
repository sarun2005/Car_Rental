using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Data;
using QuickRentMyRide.Models;
using System.Security.Claims;

namespace QuickRentMyRide.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Google Login
        [HttpGet]
        public async Task LoginWithGoogle()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse"),
                Items = { { "prompt", "select_account" } }
            };

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
        }

        
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                TempData["LoginError"] = "Google login failed.";
                return RedirectToAction("Login", "Account");
            }

            // Get Google user details
            var emailClaim = result.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var nameClaim = result.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (emailClaim == null)
            {
                TempData["LoginError"] = "Google login failed: Email not found.";
                return RedirectToAction("Login", "Account");
            }

            string email = emailClaim.Value.ToLower();

           


            // Create claims and Sign in
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nameClaim?.Value ?? email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


            TempData["LoginSuccess"] = "Welcome back!";
            return RedirectToAction("Index", "Customer");
        }



        // Facebook login
        [HttpGet]
        public async Task LoginWithFacebook()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse"),
                Items = { { "prompt", "select_account" } } 
            };

            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme, props);
        }


        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                TempData["LoginError"] = "Facebook login failed.";
                return RedirectToAction("Login", "Account");
            }

            // Get Facebook user details
            var emailClaim = result.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var nameClaim = result.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (emailClaim == null)
            {
                TempData["LoginError"] = "Facebook login failed: Email not found.";
                return RedirectToAction("Login", "Account");
            }

            string email = emailClaim.Value.ToLower();

            // Create claims and Sign in
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nameClaim?.Value ?? email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["LoginSuccess"] = "Welcome back!";
            return RedirectToAction("Index", "Customer");
        }




        //Sign Out
        [HttpPost("signout")]
        public async Task<IActionResult> signOut()
        {
            // Clear the local cookie (this handles both Google and Manual login)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }



        //Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Admin Login Check
            if (user.Email == "Admin@gmail.com" && user.Password == "Admin@123")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                // Identity & Principal creation
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["LoginSuccess"] = "Welcome Admin!";
                return RedirectToAction("Index", "Admin");
            }

            // Customer Login
            var existingUser = dbContext.Users
                .FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);

            if (existingUser != null)
            {
                TempData["LoginSuccess"] = "Welcome Customer!";
                return RedirectToAction("Index", "Customer");
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
        public IActionResult Register(Customer customer)
        {
            // email check
            if (dbContext.Users.Any(u => u.Email.ToLower() == customer.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "This email is already registered. Please login.");
                return View(customer);
            }

            // conform password
            if (customer.Password != customer.Conform_Password)
            {
                ModelState.AddModelError("Conform_Password", "Passwords do not match.");
                return View(customer);
            }


            if (ModelState.IsValid)
            {
                dbContext.Customers.Add(customer);
                dbContext.SaveChanges();

                TempData["RegisterSuccess"] = "Account created successfully! Please login.";
                return RedirectToAction("Login");
            }

            return View(customer);
        }

    }
}
