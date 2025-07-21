using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model.Entites
{
#pragma warning disable CS8618
    public class Expense
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public bool IsSpending { get; set; }
        [Required]
        public bool IsEssential { get; set; }
        public User User { get; set; }
        public string CategoryName { get; set; }
        public long UserId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public Expense(long userId, string categoryName, decimal amount, string description, bool isSpending, bool isEssential, DateTime date)
        {
            UserId = userId;
            CategoryName = categoryName;
            Amount = amount;
            IsSpending = isSpending;
            IsEssential = isEssential;
            Description = description;
            Date = date;
        }
        protected Expense() { }

    }
#pragma warning restore CS8618

}