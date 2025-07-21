using System.ComponentModel.DataAnnotations; // For ValidationException, if it's from here
using System.Security.Claims;
using System.Text.Json;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.DTOs;
using ExpenseTracker.Services;
using ExpenseTracker.Services.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("expense")]
    public class ExpenseController(ExpenseService expenseService) : ControllerBase
    {
        private readonly ExpenseService _expenseService = expenseService;
        private readonly string UnexpectedError = "An unexpected error occurred.";
        private readonly string UserError = "no logged in user";

        [HttpPost("{categoryName}/{amount}/{description}/{isSpending}/{isEssential}")]
        [Authorize]
        public async Task<IActionResult> AddExpense(string categoryName, decimal amount, string description, bool isSpending, bool isEssential)
        {
            try
            {

                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(UserError);
                long userId = long.Parse(userIdStr);

                // Await the asynchronous service call
                await _expenseService.AddExpense(userId, categoryName, amount, description, isSpending, isEssential);
                return Ok("Expense has been added successfully.");
            }
            catch (CategoryNotFoundException ex) // Catching the exception directly from the service
            {
                return NotFound(ex.Message); // Use the message from the exception
            }
            catch (ValidationException ex) // Assuming ValidationException is from System.ComponentModel.DataAnnotations or similar
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding expense: {ex.Message}"); // Log the error for debugging
                return Problem(UnexpectedError, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("{expenseId}")]
        [Authorize]
        public async Task<IActionResult> DeleteExpense(long expenseId)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(UserError);
                long userId = long.Parse(userIdStr);

                // Await the asynchronous service call
                await _expenseService.DeleteExpense(userId, expenseId);
                return Ok($"Expense with ID {expenseId} has been deleted successfully.");
            }
            catch (UnauthorizedAccessException ex) // Catching the exception directly from the service
            {
                return Unauthorized(ex.Message); // Use the message from the exception
            }
            catch (ExpenseNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting expense: {ex.Message}"); // Log the error
                return Problem(UnexpectedError, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("getExpenses")]
        [ProducesResponseType(typeof(List<ExpenseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Authorize]
        // Change to async Task<IActionResult> to properly await the service call
        public async Task<IActionResult> GetUserExpenses([FromBody] List<FilterWrapper> filterWrappers)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(UserError);
            long userId = long.Parse(userIdStr);
            Console.WriteLine($"Getting expenses for user: {userId}"); // Good for logging

            try
            {
                List<IFilterExpense> filters = FilterFactory.CreateFilters(filterWrappers);
                // Await the asynchronous service call
                List<ExpenseDto> dtos = await _expenseService.GetExpenseDtosAsync(userId, filters); // Changed to GetExpenseDtosAsync
                return Ok(dtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (JsonException ex) // For issues with JSON deserialization of filters
            {
                return BadRequest($"Invalid filter format: {ex.Message}");
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting expenses: {ex.Message}"); // Log the error
                return Problem(UnexpectedError, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateExpense([FromBody] ExpenseDto expenseDto)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdStr)) return Unauthorized(UserError);
                long userId = long.Parse(userIdStr);

                // Await the asynchronous service call
                await _expenseService.UpdateExpense(userId, expenseDto);
                return Ok("Expense has been updated successfully.");
            }
            catch (UnauthorizedAccessException ex) // Catching the exception directly from the service
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentNullException ex) // For null DTO
            {
                return BadRequest(ex.Message);
            }
            catch (ExpenseNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (CategoryNotFoundException ex) // For category not found during update
            {
                return NotFound(ex.Message);
            }
            catch (ValidationException ex) // For validation issues on the DTO/entity
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating expense: {ex.Message}"); // Log the error
                return Problem(UnexpectedError, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}