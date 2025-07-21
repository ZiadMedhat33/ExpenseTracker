using System.Text.Json;

namespace ExpenseTracker.Services.Filters
{
    public class FilterWrapper()
    {
        public string Type { get; set; } = string.Empty;
        public JsonElement Data { get; set; }

    }
    public static class FilterFactory
    {
        public static IFilterExpense Create(FilterWrapper wrapper)
        {
            switch (wrapper.Type.ToLowerInvariant())
            {
                case "category":

                    IFilterExpense? categoryFilter = wrapper.Data.Deserialize<CategoryFilter>();
                    if (categoryFilter == null)
                    {
                        throw new NotSupportedException("category type exists but its structure lead to a null object");
                    }
                    return categoryFilter;
                case "past week":
                    IFilterExpense filterWeek = new PastWeekFilter();
                    return filterWeek;
                case "past month":
                    IFilterExpense filterMonth = new PastMonthFilter();
                    return filterMonth;
                default:
                    throw new NotSupportedException("this type of filter does not exist");

            }

        }

        public static List<IFilterExpense> CreateFilters(List<FilterWrapper> wrappers)
        {
            return wrappers.Select(Create).ToList();
        }
    }
}