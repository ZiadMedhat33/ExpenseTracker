using System.Linq.Expressions;
using ExpenseTracker.Model.Entites;
using LinqKit;

namespace ExpenseTracker.Services.Filters
{
    public class CategoryFilter(string categoryName) : IFilterExpense
    {
        public string CategoryName { get; set; } = categoryName;
        public Expression<Func<Expense, bool>> ApplyFilter(Expression<Func<Expense, bool>> predicate)
        {
            return predicate.And(Expense => Expense.CategoryName == CategoryName);

        }
    }
}