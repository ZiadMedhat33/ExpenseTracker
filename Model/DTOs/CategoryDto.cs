using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model.DTOs
{
    public class CategoryDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public long UserId { get; set; } = 0; // Optional: 0 means default/global

        // Constructor
        public CategoryDto(string name, long userId = 0)
        {
            Name = name;
            UserId = userId;
        }

#pragma warning disable CS8618
        protected CategoryDto() { } // For model binding
#pragma warning restore CS8618
    }
}
