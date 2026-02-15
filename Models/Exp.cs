using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Models
{
    public class Exp
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Note { get; set; }


    }
}
