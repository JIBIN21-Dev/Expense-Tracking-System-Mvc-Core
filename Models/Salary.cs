using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Models
{
    public class Salary
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }
    }
}
