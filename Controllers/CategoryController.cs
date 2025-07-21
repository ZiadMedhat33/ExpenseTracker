

using System.Security.Claims;
using System.Threading.Tasks;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.DTOs;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("category")]
    public class CategoryController(CategoryService categoryService) : ControllerBase
    {
        private readonly string UnexpectedError = "An unexpected error occurred.";
        private readonly CategoryService _categoryService = categoryService;

        [Authorize]
        [HttpPost("{categoryName}")]
        public async Task<IActionResult> AddCategory(string categoryName)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            long userId = long.Parse(userIdStr);
            try
            {
                await _categoryService.AddCategory(userId, categoryName);
                return Ok("category added");
            }
            catch (DuplicateCategoryException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserNotFoundException)
            {
                return NotFound("user is not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem(UnexpectedError, statusCode: 500);
            }

        }

        [HttpDelete("{categoryName}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(string categoryName)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            long userId = long.Parse(userIdStr);
            try
            {
                await _categoryService.DeleteCategory(userId, categoryName);
                return Ok("category deleted");
            }
            catch (UserNotFoundException)
            {
                return NotFound("user is not found");
            }
            catch (CategoryNotFoundException)
            {
                return NotFound("category does not exist");
            }
            catch (DefualtCategoryException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem(UnexpectedError, statusCode: 500);
            }

        }

        [HttpGet("UserCategories")]
        [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetUserCategories()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            long userId = long.Parse(userIdStr);
            try
            {
                return Ok(await _categoryService.GetUserCategories(userId));
            }
            catch (UserNotFoundException)
            {
                return NotFound("user is not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem(UnexpectedError, statusCode: 500);
            }

        }


    }

}