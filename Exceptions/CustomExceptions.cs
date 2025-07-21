namespace ExpenseTracker.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
        {
        }

        public UserNotFoundException(string message)
            : base(message)
        {
        }

        public UserNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class ExpenseNotFoundException : Exception
    {
        public ExpenseNotFoundException()
        {
        }

        public ExpenseNotFoundException(string message)
            : base(message)
        {
        }

        public ExpenseNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException()
        {
        }

        public CategoryNotFoundException(string message)
            : base(message)
        {
        }

        public CategoryNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException()
        {
        }

        public UsernameAlreadyExistsException(string username)
            : base($"The username '{username}' is already taken.")
        {
        }

        public UsernameAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
    public class DuplicateCategoryException : Exception
    {
        public DuplicateCategoryException(string categoryName)
            : base($"The user already has a category named '{categoryName}'.")
        {
        }
    }

    public class DefualtCategoryException : Exception
    {
        public DefualtCategoryException()
            : base($"This is a defualt category.")
        {
        }
    }

}