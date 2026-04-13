using ApiCrud.Models;

namespace ApiCrud.Data.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
