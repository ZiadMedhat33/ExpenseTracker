using System.Linq.Expressions;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.Entites;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Model.repository
{
    public class UserRepository
    {
        // Private field to hold the injected DbContext instance
        private readonly ApplicationContext _context;

        // Constructor now takes ApplicationContext
        public UserRepository(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "ApplicationContext cannot be null.");
        }

        // Removed the Connection property as it's now handled by the injected context

        public async Task AddUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User is null");
            try
            {
                // Use the injected context
                await _context.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // For example: duplicate username/email
                // Consider logging the full dbEx here for debugging purposes
                throw new InvalidOperationException("Failed to save user to the database. It may already exist or violate constraints.", dbEx);
            }
        }

        public async Task DeleteUserById(long id)
        {
            // Use the injected context
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                throw new UserNotFoundException($"User with ID {id} not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        // Changed to async and Task<User?> for consistency and non-blocking I/O
        public async Task<User?> GetUserByIdAsync(long id)
        {
            // Use the injected context
            return await _context.Users.FindAsync(id); // returns null if not found
        }

        public async Task UpdateUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User is null");

            // Use the injected context
            var userInDb = await _context.Users.FindAsync(user.Id);
            if (userInDb is null)
            {
                throw new UserNotFoundException($"User with ID {user.Id} not found.");
            }

            userInDb.Username = user.Username;
            userInDb.Password = user.Password; // Make sure to handle password hashing/security outside the repository

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Consider logging the full dbEx here for debugging purposes
                throw new InvalidOperationException("Failed to update the user due to a database error.", dbEx);
            }
        }


        public async Task<List<User>> GetUsersAsync(Expression<Func<User, bool>> predicate)
        {

            return await _context.Users.AsExpandable().Where(predicate).ToListAsync();
        }
    }
}