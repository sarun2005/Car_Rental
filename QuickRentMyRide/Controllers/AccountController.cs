using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using QuickRentMyRide.Data;
using QuickRentMyRide.DTOs;
using QuickRentMyRide.Models;
using QuickRentMyRide.Services.Interfaces;
using System.Security.Claims;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace QuickRentMyRide.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IOtpService otpService;
        private readonly IEmailService emailService;

        public AccountController(ApplicationDbContext dbContext, IOtpService otpService, IEmailService emailService)
        {
            this.dbContext = dbContext;
            this.otpService = otpService;
            this.emailService = emailService;
        }

        // ============ Register GET ===========
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ============ Register POST with OTP send ===========
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return View(registerDto);

            // Check email duplicate
            if (dbContext.Users.Any(u => u.Email.ToLower() == registerDto.Email.ToLower()))
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(registerDto);
            }

            // Generate OTP and save in session
            var otp = otpService.GenerateOtp();
            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("Email", registerDto.Email);
            HttpContext.Session.SetString("FullName", registerDto.Full_Name);
            HttpContext.Session.SetString("Password", registerDto.Password);

            string emailBody = $"Your OTP code is: <b>{otp}</b>";
            await emailService.SendEmailAsync(registerDto.Email, "OTP Verification", emailBody);

            return RedirectToAction("VerifyOtp");
        }

        // ============ Verify OTP GET ===========
        [HttpGet]
        public IActionResult VerifyOtp()
        {
            return View();
        }

        // ============ Verify OTP POST ===========
        [HttpPost]
        public IActionResult VerifyOtp(string inputOtp)
        {
            var actualOtp = HttpContext.Session.GetString("OTP");
            var email = HttpContext.Session.GetString("Email");
            var fullName = HttpContext.Session.GetString("FullName");
            var password = HttpContext.Session.GetString("Password");

            if (inputOtp == actualOtp)
            {
                // Hash password before saving
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                var user = new User
                {
                    Email = email,
                    Password = hashedPassword,
                    Role = "Customer"
                };
                dbContext.Users.Add(user);
                dbContext.SaveChanges();

                var customer = new Customer
                {
                    Email = email,
                    Full_Name = fullName,
                    UserID = user.UserID
                };
                dbContext.Customers.Add(customer);
                dbContext.SaveChanges();

                HttpContext.Session.Clear();

                TempData["RegisterSuccess"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError("", "Invalid OTP. Please try again.");
                return View();
            }
        }

        // ============ Login GET ===========
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ============ Login POST ===========
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Admin login (hardcoded)
            if (user.Email.ToLower() == "admin@gmail.com" && user.Password == "Admin@123")
            {
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Admin"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var adminIdentity = new ClaimsIdentity(adminClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                var adminPrincipal = new ClaimsPrincipal(adminIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, adminPrincipal);
                TempData["LoginSuccess"] = "Welcome Admin!";
                return RedirectToAction("Index", "Admin");
            }

            // Find user by email
            var existingUser = dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == user.Email.ToLower());
            if (existingUser != null)
            {
                // Verify hashed password
                bool passwordValid = BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password);
                if (passwordValid)
                {
                    var customerClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, existingUser.Email),
                        new Claim(ClaimTypes.Email, existingUser.Email),
                        new Claim(ClaimTypes.Role, "Customer")
                    };

                    var customerIdentity = new ClaimsIdentity(customerClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var customerPrincipal = new ClaimsPrincipal(customerIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, customerPrincipal);
                    TempData["LoginSuccess"] = "Welcome Customer!";
                    return RedirectToAction("Index", "Customer");
                }
            }

            ModelState.AddModelError("", "Invalid Email or Password.");
            return View(user);
        }

        // ============ Logout POST ===========
        [HttpPost("signout")]
        public async Task<IActionResult> signOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ============ Google Login ===========
        [HttpGet]
        public async Task LoginWithGoogle()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, props);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                TempData["LoginError"] = "Google login failed.";
                return RedirectToAction("Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Google login failed: Email not found.";
                return RedirectToAction("Login");
            }

            // Optionally: check if user exists in DB; if not, create new user
            var user = dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    Password = "", // No password for social login or set random
                    Role = "Customer"
                };
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name ?? email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            TempData["LoginSuccess"] = "Logged in with Google!";
            return RedirectToAction("Index", "Customer");
        }

        // ============ Facebook Login ===========
        [HttpGet]
        public async Task LoginWithFacebook()
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action("FacebookResponse")
            };
            await HttpContext.ChallengeAsync(FacebookDefaults.AuthenticationScheme, props);
        }

        public async Task<IActionResult> FacebookResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                TempData["LoginError"] = "Facebook login failed.";
                return RedirectToAction("Login");
            }

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                TempData["LoginError"] = "Facebook login failed: Email not found.";
                return RedirectToAction("Login");
            }

            // Optionally: check if user exists in DB; if not, create new user
            var user = dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    Password = "", // No password for social login or set random
                    Role = "Customer"
                };
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name ?? email),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, "Customer")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            TempData["LoginSuccess"] = "Logged in with Facebook!";
            return RedirectToAction("Index", "Customer");
        }
    }
}
