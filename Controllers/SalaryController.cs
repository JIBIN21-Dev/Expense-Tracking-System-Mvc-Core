using Microsoft.AspNetCore.Mvc;
using ExpenseTracking.Data;
using ExpenseTracking.Models;

namespace ExpenseTracking.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ExpDbContext _context;

        public SalaryController(ExpDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SaveSalary(decimal amount)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var existingSalary = _context.Salaries
                .FirstOrDefault(s => s.UserId == userId &&
                                    s.Year == currentYear &&
                                    s.Month == currentMonth);

            if (existingSalary == null)
            {
                var newSalary = new Salary
                {
                    UserId = userId.Value,
                    Amount = amount,
                    Year = currentYear,
                    Month = currentMonth
                };
                _context.Salaries.Add(newSalary);
            }
            else
            {
                existingSalary.Amount = amount;
            }

            _context.SaveChanges();

            return Ok(new { success = true, message = "Salary saved successfully" });
        }
    }
}