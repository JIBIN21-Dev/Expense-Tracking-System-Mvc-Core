using Microsoft.AspNetCore.Mvc;
using ExpenseTracking.Data;
using ExpenseTracking.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracking.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ExpDbContext _context;

        public ExpenseController(ExpDbContext context)
        {
            _context = context;
        }

        // Add new expense
        [HttpPost]
        public IActionResult AddExpense(Exp exp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            exp.UserId = userId.Value;
            _context.Exp.Add(exp);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Expense added successfully" });
        }

        // Get all expenses for current user
        [HttpGet]
        public IActionResult GetExpenses()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var expenses = _context.Exp
                .Where(e => e.UserId == userId.Value)
                .OrderByDescending(e => e.Date)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Amount,
                    e.Category,
                    e.Date,
                    e.Note
                })
                .ToList();

            return Json(expenses);
        }

        // Get single expense by ID
        [HttpGet]
        public IActionResult GetExpense(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var expense = _context.Exp
                .Where(e => e.Id == id && e.UserId == userId.Value)
                .Select(e => new
                {
                    e.Id,
                    e.Name,
                    e.Amount,
                    e.Category,
                    e.Date,
                    e.Note
                })
                .FirstOrDefault();

            if (expense == null)
                return NotFound();

            return Json(expense);
        }

        // Update expense
        [HttpPost]
        public IActionResult UpdateExpense(Exp exp)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var existingExpense = _context.Exp
                .FirstOrDefault(e => e.Id == exp.Id && e.UserId == userId.Value);

            if (existingExpense == null)
                return NotFound();

            existingExpense.Name = exp.Name;
            existingExpense.Amount = exp.Amount;
            existingExpense.Category = exp.Category;
            existingExpense.Date = exp.Date;
            existingExpense.Note = exp.Note;

            _context.SaveChanges();

            return Ok(new { success = true, message = "Expense updated successfully" });
        }

        // Delete expense
        [HttpDelete]
        public IActionResult DeleteExpense(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var expense = _context.Exp
                .FirstOrDefault(e => e.Id == id && e.UserId == userId.Value);

            if (expense == null)
                return NotFound();

            _context.Exp.Remove(expense);
            _context.SaveChanges();

            return Ok(new { success = true, message = "Expense deleted successfully" });
        }

        // Clear all expenses
        [HttpDelete]
        public IActionResult ClearAll()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return Unauthorized();

            var expenses = _context.Exp
                .Where(e => e.UserId == userId.Value)
                .ToList();

            _context.Exp.RemoveRange(expenses);
            _context.SaveChanges();

            return Ok(new { success = true, message = "All expenses cleared" });
        }
    }
}