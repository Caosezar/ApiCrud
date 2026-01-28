using ApiCrud.Models;

namespace ApiCrud.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int id);
    }
}
