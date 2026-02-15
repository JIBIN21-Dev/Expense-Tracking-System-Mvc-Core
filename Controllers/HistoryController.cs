using Microsoft.AspNetCore.Mvc;
using ExpenseTracking.Data;

public class HistoryController : Controller
{
    private readonly ExpDbContext _context;

    public HistoryController(ExpDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        var salaries = _context.Salaries
            .Where(s => s.UserId == userId)
            .ToList();

        var expenses = _context.Exp
            .Where(e => e.UserId == userId)
            .ToList();

        var history = salaries.Select(s => new
        {
            Month = $"{s.Month}/{s.Year}",
            Salary = s.Amount,
            Expenses = expenses
                .Where(e => e.Date.Month == s.Month && e.Date.Year == s.Year)
                .Sum(e => e.Amount)
        })
        .Select(h => new
        {
            h.Month,
            h.Salary,
            h.Expenses,
            Savings = h.Salary - h.Expenses
        })
        .OrderByDescending(h => h.Month)
        .ToList();

        return View(history);
    }
}