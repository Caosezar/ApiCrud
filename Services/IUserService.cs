using ApiCrud.Models;

namespace ApiCrud.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(int id);

        Task<User> UpdateUserAsync(int id, User user);
    }
}
