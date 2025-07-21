using System.Threading.Tasks;
using ExpenseTracker.Exceptions;
using ExpenseTracker.Model.DTOs;
using ExpenseTracker.Model.Entites;
using ExpenseTracker.Model.repository;

namespace ExpenseTracker.Services
{
    public class UserService(UserRepository userRepository)
    {
        public UserRepository UserRepository { get; set; } = userRepository;

        public static User ToEntity(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }
            User User = new(userDto.Username, userDto.Password)
            {
                Id = userDto.Id
            };
            return User;
        }
        public static UserDto ToDto(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return new UserDto
            (

                username: user.Username,
                password: user.Password
            )
            {
                Id = user.Id
            };
        }
        public static CategoryDto ToDto(Category entity)
        {
            return new CategoryDto(entity.Name, entity.UserId);
        }
        public async Task validateUserName(string username)
        {
            User? User = (await UserRepository.GetUsersAsync(User => User.Username == username)).FirstOrDefault();
            if (User != null)
            {
                throw new UsernameAlreadyExistsException(username);
            }
        }
        public async Task Register(string username, string password)
        {
            User User = new(username, password);
            await validateUserName(username);
            ValidationHelper.ValidateEntity(User);
            await UserRepository.AddUser(User);
        }
        public async Task Update(long userId, UserDto userDto)
        {
            User User = ToEntity(userDto);
            if (userId != userDto.Id)
            {
                throw new UnauthorizedAccessException();
            }
            await validateUserName(User.Username);
            ValidationHelper.ValidateEntity(User);
            await UserRepository.UpdateUser(User);
        }
        public static async Task<bool> UserExists(long userId, UserRepository rep)
        {
            User? User = await rep.GetUserByIdAsync(userId);
            if (User == null) return false;
            return true;
        }
        public async Task<UserDto> Login(string username, string password)
        {
            User? User = (await UserRepository.GetUsersAsync(User => User.Username == username && User.Password == password)).FirstOrDefault();
            if (User == null)
            {
                throw new UserNotFoundException();
            }
            return ToDto(User);
        }
    }
}