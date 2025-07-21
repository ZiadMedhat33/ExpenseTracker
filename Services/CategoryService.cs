using ExpenseTracker.Model.repository;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.DTOs;
using ExpenseTracker.Model.Entites;
using System.Threading.Tasks;
namespace ExpenseTracker.Services
{
    public class CategoryService(CategoryRepository categoryRepository, UserRepository userRepository)
    {
        public CategoryRepository CategoryRepository { get; set; } = categoryRepository;
        public UserRepository UserRepository { get; set; } = userRepository;

        public static Category ToEntity(CategoryDto dto)
        {
            return new Category(dto.Name, dto.UserId);
        }
        public static List<Category> ToEntity(List<CategoryDto> dtos) =>
        dtos.Select(ToEntity).ToList();
        public static CategoryDto ToDto(Category entity)
        {
            return new CategoryDto(entity.Name, entity.UserId);
        }
        public static List<CategoryDto> ToDto(List<Category> entities) =>
        entities.Select(ToDto).ToList();
        public static async Task<bool> CategoryExistsAsync(long userId, string CategoryName, CategoryRepository categoryRepository)
        {
            List<string> defaultCategoryList = Enum.GetNames(typeof(DefaultCategories)).ToList();
            Category? Category = (await categoryRepository.GetCategoriesAsync(category => (category.UserId == userId)
            && category.Name == CategoryName)).FirstOrDefault();
            return Category != null || defaultCategoryList.Contains(CategoryName);
        }
        public static bool DefualtCategory(string CategoryName)
        {
            List<string> defaultCategoryList = Enum.GetNames(typeof(DefaultCategories)).ToList();
            return defaultCategoryList.Contains(CategoryName);

        }

        public async Task AddCategory(long userId, string name)
        {
            if (!await UserService.UserExists(userId, UserRepository))
            {
                throw new UserNotFoundException();
            }
            if (await CategoryExistsAsync(userId, name, CategoryRepository))
            {
                throw new DuplicateCategoryException(name);
            }
            Category category = new Category(name, userId);
            await CategoryRepository.AddCategory(category);
        }

        public async Task DeleteCategory(long userId, string name)
        {
            if (!await CategoryExistsAsync(userId, name, CategoryRepository))
            {
                throw new CategoryNotFoundException(name);
            }
            if (DefualtCategory(name))
            {
                throw new DefualtCategoryException();
            }
            await CategoryRepository.DeleteCategoryByKey(userId, name);
        }
        public async Task<List<CategoryDto>> GetUserCategories(long userId)
        {
            if (!await UserService.UserExists(userId, UserRepository))
            {
                throw new UserNotFoundException();
            }
            List<Category> categories = await CategoryRepository.GetCategoriesAsync(Category => Category.UserId == userId);
            List<string> defaultCategoryList = Enum.GetNames(typeof(DefaultCategories)).ToList();
            foreach (string name in defaultCategoryList)
            {
                categories.Add(new Category(name, 0));
            }
            return ToDto(categories);

        }

    }
}