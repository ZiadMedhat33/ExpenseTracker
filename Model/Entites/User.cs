using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model.Entites
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        [StringLength(30, MinimumLength = 5)]
        [Required]
        public string Username { get; set; }

        [StringLength(64, MinimumLength = 8)]
        [Required]
        public string Password { get; set; }

        public ICollection<Expense> Expenses { get; set; } = [];
        public ICollection<Category> Categories { get; set; } = [];

        public User(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
#pragma warning disable CS8618 // Non-nullable property 'Username' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        protected User() { }
#pragma warning restore CS8618

    }
}