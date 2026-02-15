using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Models;




namespace ExpenseTracking.Data
{
    public class ExpDbContext: DbContext
    {
        public ExpDbContext(DbContextOptions<ExpDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Exp> Exp { get; set; }

        public DbSet<Salary> Salaries { get; set; }


    }
}
