using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Model.DTOs
{
    public class UserDto
    {
        public long Id { get; set; } // Include for response; omit for create if needed

        [Required]
        [StringLength(30, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; set; }

        public UserDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
#pragma warning disable CS8618
        protected UserDto() { }
#pragma warning restore CS8618
    }
}
