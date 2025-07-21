using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.DTOs;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Controllers
{
    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public UserController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }
        [HttpPost("register/{username}/{password}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(string username, string password)
        {
            try
            {
                await _userService.Register(username, password);
                return Ok("user has been added sucessfully");
            }
            catch (UsernameAlreadyExistsException)
            {
                return Conflict("Username already taken");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem("An unexpected error occurred.", statusCode: 500);
            }
        }
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("login/{username}/{password}")]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                UserDto user = await _userService.Login(username, password);
                string token = _jwtService.GenerateToken(user);
                return Ok(new
                {
                    Token = token,
                    message = "Use this token in the Authorize button at the top right."
                });
            }
            catch (UserNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem("An unexpected error occurred.", statusCode: 500);
            }
        }
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            long userId = long.Parse(userIdStr);
            try
            {
                await _userService.Update(userId, dto);
                return Ok("User has been updated");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("user logged in is not the user being updated");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("object sent is null");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem("An unexpected error occurred.", statusCode: 500);
            }

        }
    }
}