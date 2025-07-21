using System.Linq.Expressions;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.Entites;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Model.repository
{
    public class ExpenseRepository
    {
        // Private field to hold the injected DbContext instance
        private readonly ApplicationContext _context;

        // Constructor now takes ApplicationContext
        public ExpenseRepository(ApplicationContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "ApplicationContext cannot be null.");
        }

        // Removed the Connection property as it's now handled by the injected context

        public async Task AddExpense(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense), "Expense is null");

            try
            {
                await _context.AddAsync(expense);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Consider logging the full dbEx here for debugging purposes
                throw new InvalidOperationException("Failed to save expense to the database.", dbEx);
            }
        }

        public async Task DeleteExpenseById(long id)
        {
            // Use the injected context
            var expense = await _context.Expenses.FindAsync(id);
            if (expense is null)
            {
                throw new ExpenseNotFoundException($"Expense with ID {id} not found.");
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }

        // Changed to async and Task<Expense?> for consistency and non-blocking I/O
        public async Task<Expense?> GetExpenseByIdAsync(long id)
        {
            // Use the injected context
            return await _context.Expenses.FindAsync(id); // returns null if not found
        }

        public async Task UpdateExpense(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense), "Expense is null");

            // Use the injected context
            var expenseInDb = await _context.Expenses.FindAsync(expense.Id);
            if (expenseInDb is null)
            {
                throw new ExpenseNotFoundException($"Expense with ID {expense.Id} not found.");
            }

            // Update properties of the tracked entity
            expenseInDb.CategoryName = expense.CategoryName;
            expenseInDb.Amount = expense.Amount;
            expenseInDb.IsEssential = expense.IsEssential;
            expenseInDb.IsSpending = expense.IsSpending;
            expenseInDb.UserId = expense.UserId;
            expenseInDb.Description = expense.Description;
            expenseInDb.Date = expense.Date;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Consider logging the full dbEx here for debugging purposes
                throw new InvalidOperationException("Failed to update the expense due to a database error.", dbEx);
            }
        }


        public async Task<List<Expense>> GetExpensesAsync(Expression<Func<Expense, bool>> predicate)
        {

            return await _context.Expenses.AsExpandable().Where(predicate).ToListAsync();
        }
    }
}