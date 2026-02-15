using Microsoft.AspNetCore.Mvc;
using ExpenseTracking.Data;
using System.Linq;

namespace ExpenseTracking.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ExpDbContext _context;

        public DashboardController(ExpDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // Get current month expenses
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var expenses = _context.Exp
                .Where(e => e.UserId == userId.Value &&
                           e.Date.Month == currentMonth &&
                           e.Date.Year == currentYear)
                .ToList();

            // Get current salary
            var salary = _context.Salaries
                .Where(s => s.UserId == userId.Value &&
                           s.Month == currentMonth &&
                           s.Year == currentYear)
                .FirstOrDefault();

            var salaryAmount = salary?.Amount ?? 0;
            var totalExpenses = expenses.Sum(e => e.Amount);
            var monthlySavings = salaryAmount - totalExpenses;

            ViewBag.Salary = salaryAmount;
            ViewBag.TotalExpenses = totalExpenses;
            ViewBag.MonthlySavings = monthlySavings;
            ViewBag.ExpenseCount = expenses.Count;

            // Get monthly history - FIX: Get data first, THEN format
            var salaryData = _context.Salaries
                .Where(s => s.UserId == userId.Value)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .Take(6)
                .ToList(); // Execute query first

            var history = salaryData.Select(s => new
            {
                Month = $"{GetMonthName(s.Month)} {s.Year}", // Now it's in memory, safe to use
                Salary = s.Amount,
                Expenses = _context.Exp
                    .Where(e => e.UserId == userId.Value &&
                               e.Date.Month == s.Month &&
                               e.Date.Year == s.Year)
                    .Sum(e => (decimal?)e.Amount) ?? 0,
            })
            .Select(h => new
            {
                h.Month,
                h.Salary,
                h.Expenses,
                Savings = h.Salary - h.Expenses
            })
            .ToList();

            ViewBag.History = history;

            return View();
        }

        [HttpGet]
        public IActionResult GetTotals()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var expenses = _context.Exp
                .Where(e => e.UserId == userId.Value &&
                           e.Date.Month == currentMonth &&
                           e.Date.Year == currentYear)
                .ToList();

            var totalExpenses = expenses.Sum(e => e.Amount);

            var salary = _context.Salaries
                .Where(s => s.UserId == userId.Value &&
                           s.Month == currentMonth &&
                           s.Year == currentYear)
                .Select(s => s.Amount)
                .FirstOrDefault();

            var savings = salary - totalExpenses;

            return Json(new
            {
                total = totalExpenses,
                count = expenses.Count,
                salary = salary,
                savings = savings
            });
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            // FIX: Get data first, THEN format
            var salaryData = _context.Salaries
                .Where(s => s.UserId == userId.Value)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .Take(6)
                .ToList(); // Execute query first

            var history = salaryData.Select(s => new
            {
                month = $"{GetMonthName(s.Month)} {s.Year}", // Now safe to use
                salary = s.Amount,
                expenses = _context.Exp
                    .Where(e => e.UserId == userId.Value &&
                               e.Date.Month == s.Month &&
                               e.Date.Year == s.Year)
                    .Sum(e => (decimal?)e.Amount) ?? 0
            })
            .Select(h => new
            {
                h.month,
                h.salary,
                h.expenses,
                savings = h.salary - h.expenses
            })
            .ToList();

            return Json(history);
        }

        private string GetMonthName(int month)
        {
            return new DateTime(2000, month, 1).ToString("MMM");
        }
    }
}