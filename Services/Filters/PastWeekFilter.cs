using System.Linq.Expressions;
using ExpenseTracker.Model.Entites;
using LinqKit;

namespace ExpenseTracker.Services.Filters
{
    public class PastWeekFilter : IFilterExpense
    {
        public Expression<Func<Expense, bool>> ApplyFilter(Expression<Func<Expense, bool>> predicate)
        {
            DateTime Date = DateTime.UtcNow.AddDays(-7);
            return predicate.And(Expense => Expense.Date > Date);

        }
    }
}