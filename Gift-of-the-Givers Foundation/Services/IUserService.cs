using Gift_of_the_Givers_Foundation.Models;

namespace Gift_of_the_Givers_Foundation.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user, string password);
        Task<User> AuthenticateUserAsync(string email, string password);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string email);
    }
}