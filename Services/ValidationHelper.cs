using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker
{
    public static class ValidationHelper
    {
        public static void ValidateEntity(object entity)
        {
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, context, results, validateAllProperties: true);

            if (!isValid)
            {
                var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }
        }
    }
}
