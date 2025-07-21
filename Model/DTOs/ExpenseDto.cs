using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model.DTOs
{
    public class ExpenseDto
    {
        public long Id { get; set; } // Include if used for response. Remove for creation requests.

        [Required]
        public long UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public bool IsSpending { get; set; }

        [Required]
        public bool IsEssential { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public ExpenseDto(long userId, string categoryName, decimal amount, string description, bool isSpending, bool isEssential, DateTime date)
        {
            UserId = userId;
            CategoryName = categoryName;
            Amount = amount;
            IsSpending = isSpending;
            IsEssential = isEssential;
            Description = description;
            Date = date;
        }
#pragma warning disable CS8618
        protected ExpenseDto() { }
#pragma warning restore CS8618
    }
}
