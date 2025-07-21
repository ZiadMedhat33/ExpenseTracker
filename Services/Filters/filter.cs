using System.Linq.Expressions;
using ExpenseTracker.Model.Entites;

namespace ExpenseTracker.Services.Filters
{
    public interface IFilterExpense
    {
        public Expression<Func<Expense, bool>> ApplyFilter(Expression<Func<Expense, bool>> predicate);
    }
}