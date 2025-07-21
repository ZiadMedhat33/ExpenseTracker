using System.Linq.Expressions;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.Entites;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Model.repository
{
    public class CategoryRepository(ApplicationContext context)
    {


        public async Task AddCategory(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Category is null");

            try
            {
                await context.AddAsync(category);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Failed to save category to the database. It may already exist or violate constraints.", dbEx);
            }
        }

        public async Task DeleteCategoryByKey(long userId, string name)
        {
            var category = await context.Categories.FindAsync(userId, name);
            if (category is null)
            {
                throw new CategoryNotFoundException($"Category '{name}' not found for user ID {userId}.");
            }

            context.Categories.Remove(category);
            await context.SaveChangesAsync();
        }

        public Category? GetCategoryByKey(long userId, string name)
        {
            return context.Categories.Find(userId, name);
        }

        public async Task UpdateCategory(Category updatedCategory)
        {
            if (updatedCategory == null)
                throw new ArgumentNullException(nameof(updatedCategory), "Category is null");

            var category = await context.Categories.FindAsync(updatedCategory.UserId, updatedCategory.Name);
            if (category is null)
            {
                throw new CategoryNotFoundException($"Category '{updatedCategory.Name}' not found for user ID {updatedCategory.UserId}.");
            }

            category.Name = updatedCategory.Name;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new InvalidOperationException("Failed to update the category due to a database error.", dbEx);
            }
        }

        public async Task<List<Category>> GetCategoriesAsync(Expression<Func<Category, bool>> predicate)
        {
            return await context.Categories.AsExpandable().Where(predicate).ToListAsync();
        }
    }
}
