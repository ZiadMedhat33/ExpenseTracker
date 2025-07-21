using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model.Entites
{

    public class Category
    {
#pragma warning disable CS8618
        public long UserId { get; set; } // if the user Id is 0 it means its default
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public Category(string name, long userId = 0)
        {
            Name = name;
            UserId = userId;
        }
        protected Category() { }
        public User User { get; set; }
#pragma warning restore CS8618
    }
    public enum DefaultCategories
    {
        Groceries,
        Leisure,
        Electronics,
        Utilities,
        Clothing,
        Health
    }
}