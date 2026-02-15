using ExpenseTracking.Data;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracking.Models;
using System.Linq;
using System.Security.Cryptography;

namespace ExpenseTracking.Controllers
{
    public class AccountController : Controller
    {
        private readonly ExpDbContext _context;

        public AccountController(ExpDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Users user)
        {
            if (ModelState.IsValid)
            {
                user.Password = HashPassword(user.Password);
                _context.Users.Add(user);
                _context.SaveChanges();

                TempData["Success"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }
            return View(user);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            var user = _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

            if (user != null)
            {

                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);

                TempData["Success"] = "Login successful.";
                return RedirectToAction("Dashboard");
            }

            TempData["Error"] = "Invalid email or password.";
            return RedirectToAction("Login");
        }
        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }
        // FORGOT PASSWORD
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email, string newPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user != null)
            {
                user.Password = HashPassword(newPassword);
                _context.SaveChanges();

                TempData["Success"] = "Password reset successful.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Email not found.";
            return RedirectToAction("ForgotPassword");
        }


        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToAction("Login", "Account");
            }


            return View();
        }
    }
}