using System.Linq.Expressions;
using System.Threading.Tasks; // Explicitly added for clarity
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.DTOs;
using ExpenseTracker.Model.Entites;
using ExpenseTracker.Model.repository;
using ExpenseTracker.Services.Filters;

namespace ExpenseTracker.Services
{
    // The constructor already correctly injects the repositories
    public class ExpenseService(ExpenseRepository expenseRepository, UserRepository userRepository, CategoryRepository categoryRepository)
    {
        // Public properties for repositories are usually fine in a service layer,
        // though some prefer to keep them private and only expose methods.
        public ExpenseRepository ExpenseRepository { get; set; } = expenseRepository;
        public UserRepository UserRepository { get; set; } = userRepository;
        public CategoryRepository CategoryRepository { get; set; } = categoryRepository;

        // --- DTO to Entity and Entity to DTO Mappers ---
        // These static methods are good for separation of concerns.

        public static Expense ToEntity(ExpenseDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            return new Expense(
                userId: dto.UserId,
                categoryName: dto.CategoryName,
                amount: dto.Amount,
                description: dto.Description,
                isSpending: dto.IsSpending,
                isEssential: dto.IsEssential,
                date: dto.Date
            )
            {
                Id = dto.Id,
            };
        }

        public static List<Expense> ToEntity(List<ExpenseDto> dtos) =>
            dtos.Select(ToEntity).ToList();

        public static ExpenseDto ToDto(Expense entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            return new ExpenseDto
            (
                userId: entity.UserId,
                categoryName: entity.CategoryName,
                amount: entity.Amount,
                description: entity.Description,
                isSpending: entity.IsSpending,
                isEssential: entity.IsEssential,
                date: entity.Date
            )
            {
                Id = entity.Id,
            };
        }

        public static List<ExpenseDto> ToDto(List<Expense> entities) =>
            entities.Select(ToDto).ToList();

        // --- Service Logic Methods ---

        /// <summary>
        /// Adds a new expense after validating the user and category exist.
        /// </summary>
        public async Task AddExpense(long userId, string categoryName, decimal amount,
                                     string description, bool isSpending, bool isEssential)
        {
            // Assuming UserService.UserExists has been updated to be async
            // and CategoryService.CategoryExists has been updated to be async.
            // If these are static helper methods, they should also be updated to call async repository methods.
            if (!await UserService.UserExistsAsync(userId, UserRepository)) // Await the async UserExists method
            {
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            if (!await CategoryService.CategoryExistsAsync(userId, categoryName, CategoryRepository)) // Await the async CategoryExists method
            {
                throw new CategoryNotFoundException($"Category '{categoryName}' not found for user ID {userId}.");
            }

            Expense NewExpense = new(userId, categoryName, amount, description, isSpending, isEssential, DateTime.UtcNow);
            ValidationHelper.ValidateEntity(NewExpense); // Assuming this remains synchronous
            await ExpenseRepository.AddExpense(NewExpense);
        }

        /// <summary>
        /// Deletes an expense for a specific user after ownership validation.
        /// </summary>
        public async Task DeleteExpense(long userId, long expenseId)
        {
            // Use the asynchronous GetExpenseByIdAsync method
            Expense? Expense = await ExpenseRepository.GetExpenseByIdAsync(expenseId);
            if (Expense == null)
            {
                throw new ExpenseNotFoundException($"Expense with ID {expenseId} does not exist.");
            }
            if (Expense.UserId != userId)
            {
                throw new UnauthorizedAccessException("Logged-in user does not own this expense.");
            }
            await ExpenseRepository.DeleteExpenseById(expenseId);
        }

        /// <summary>
        /// Checks if an expense exists by its ID.
        /// </summary>
        /// <remarks>
        /// This method should also be asynchronous to match the repository.
        /// Changed from static to instance method if it relies on the injected repository.
        /// Alternatively, if it remains static, `rep.GetExpenseByIdAsync` needs to be awaited.
        /// For simplicity and common practice, making it an instance method for consistency.
        /// </remarks>
        public async Task<bool> ExpenseExistsAsync(long id)
        {
            Expense? expense = await ExpenseRepository.GetExpenseByIdAsync(id);
            return expense != null;
        }

        /// <summary>
        /// Updates an existing expense after validation and ownership checks.
        /// </summary>
        public async Task UpdateExpense(long userId, ExpenseDto dto)
        {
            Expense expenseToUpdate = ToEntity(dto); // Renamed variable to avoid conflict
            // Use the asynchronous ExpenseExistsAsync method
            if (!await ExpenseExistsAsync(dto.Id))
            {
                throw new ExpenseNotFoundException($"Expense with ID {dto.Id} not found.");
            }
            if (userId != dto.UserId)
            {
                throw new UnauthorizedAccessException("Logged-in user does not own this expense.");
            }
            ValidationHelper.ValidateEntity(expenseToUpdate); // Assuming this remains synchronous

            // Ensure the category still exists for the user after potential category name changes in DTO
            if (!await CategoryService.CategoryExistsAsync(userId, expenseToUpdate.CategoryName, CategoryRepository))
            {
                throw new CategoryNotFoundException($"Category '{expenseToUpdate.CategoryName}' not found for user {userId}.");
            }

            await ExpenseRepository.UpdateExpense(expenseToUpdate);
        }

        /// <summary>
        /// Retrieves a list of expense DTOs for a given user, applying specified filters.
        /// </summary>
        public async Task<List<ExpenseDto>> GetExpenseDtosAsync(long userId, List<IFilterExpense> filters)
        {
            // Assuming UserService.UserExists has been updated to be async
            if (!await UserService.UserExistsAsync(userId, UserRepository))
            {
                throw new UserNotFoundException($"User with ID {userId} not found.");
            }
            Expression<Func<Expense, bool>> predicate = a => a.UserId == userId; // Start with user-specific filter
            foreach (IFilterExpense filter in filters)
            {
                predicate = filter.ApplyFilter(predicate);
            }
            // Await the asynchronous repository call
            return ToDto(await ExpenseRepository.GetExpensesAsync(predicate));
        }

        // --- Placeholder for UserService and CategoryService methods ---
        // You'll need to define these somewhere, likely in separate service files.
        // They should also be updated to use their respective repository's async methods.
        public static class UserService
        {
            // Assuming UserRepository.GetUserByIdAsync is available
            public static async Task<bool> UserExistsAsync(long userId, UserRepository userRepository)
            {
                var user = await userRepository.GetUserByIdAsync(userId);
                return user != null;
            }
        }
    }
}